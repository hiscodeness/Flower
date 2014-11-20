using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Flower.Works
{
    internal class ActionWorkObservablesHelper : WorkObservablesHelper<IRegisteredActionWork, ITriggeredActionWork, IExecutableActionWork>
    {
        public ActionWorkObservablesHelper(IRegisteredActionWork work)
            : base(work)
        {
        }
    }

    internal class ActionWorkObservablesHelper<TInput> : WorkObservablesHelper<IRegisteredActionWork<TInput>, ITriggeredActionWork<TInput>, IExecutableActionWork<TInput>>
    {
        public ActionWorkObservablesHelper(IRegisteredActionWork<TInput> work)
            : base(work)
        {
        }
    }

    internal class FuncWorkObservablesHelper<TInput, TOutput> : WorkObservablesHelper<IRegisteredFuncWork<TInput, TOutput>, ITriggeredFuncWork<TInput, TOutput>, IExecutableFuncWork<TInput, TOutput>>
    {
        public FuncWorkObservablesHelper(IRegisteredFuncWork<TInput, TOutput> work)
            : base(work)
        {
        } 
    }

    internal class WorkObservablesHelper<TWork, TTriggeredWork, TExecutableWork>
        where TWork : IRegisteredWork where TTriggeredWork : ITriggeredWork where TExecutableWork : IExecutableWork
    {
        private readonly WorkTriggeredHelper workTriggeredHelper;
        private readonly WorkExecutedHelper workExecutedHelper;

        protected WorkObservablesHelper(TWork work)
        {
            workTriggeredHelper = new WorkTriggeredHelper();
            workExecutedHelper = new WorkExecutedHelper(work);
        }

        public IObservable<TTriggeredWork> WorkTriggered { get { return workTriggeredHelper.WorkTriggered; } }
        public IObservable<TExecutableWork> WorkExecuted { get { return workExecutedHelper.WorkExecuted; } }

        public void RaiseWorkTriggered(TTriggeredWork triggeredWork)
        {
            workTriggeredHelper.RaiseWorkTriggered(triggeredWork);
        }

        public void RaiseWorkExecuted(TExecutableWork triggeredWork)
        {
            workExecutedHelper.RaiseWorkExecuted(triggeredWork);
        }

        public void RaiseWorkCompleted()
        {
            workTriggeredHelper.RaiseWorkCompleted();
            workExecutedHelper.RaiseWorkCompleted();
        }

        private class WorkCompletedHelper
        {
            protected event Action workCompleted;
            
            public void RaiseWorkCompleted()
            {
                OnWorkCompleted();
            }

            private void OnWorkCompleted()
            {
                var handler = workCompleted;
                if (handler != null)
                {
                    handler();
                }
            }
        }

        private class WorkTriggeredHelper : WorkCompletedHelper
        {
            private event Action<TTriggeredWork> workTriggered;

            public WorkTriggeredHelper()
            {
                WorkTriggered = Observable.Create(CreateTriggeredSubscription());
            }

            public IObservable<TTriggeredWork> WorkTriggered { get; private set; }

            public void RaiseWorkTriggered(TTriggeredWork triggeredWork)
            {
                OnWorkTriggered(triggeredWork);
            }

            private Func<IObserver<TTriggeredWork>, IDisposable> CreateTriggeredSubscription()
            {
                return CreateTriggeredSubscription;
            }

            private IDisposable CreateTriggeredSubscription(IObserver<TTriggeredWork> observer)
            {
                workTriggered += observer.OnNext;
                workCompleted += observer.OnCompleted;
                return Disposable.Create(() =>
                    {
                        workTriggered -= observer.OnNext;
                        workCompleted -= observer.OnCompleted;
                    });
            }

            private void OnWorkTriggered(TTriggeredWork triggeredWork)
            {
                var handler = workTriggered;
                if (handler != null)
                {
                    handler(triggeredWork);
                }
            }
        }

        private class WorkExecutedHelper : WorkCompletedHelper
        {
            private readonly TWork work;
            private event Action<TExecutableWork> workExecuted;

            public WorkExecutedHelper(TWork work)
            {
                this.work = work;
                WorkExecuted = Observable.Create(CreateExecutedSubscription());
            }

            public IObservable<TExecutableWork> WorkExecuted { get; private set; }

            public void RaiseWorkExecuted(TExecutableWork triggeredWork)
            {
                OnWorkExecuted(triggeredWork);
            }

            private Func<IObserver<TExecutableWork>, IDisposable> CreateExecutedSubscription()
            {
                return CreateExecutedSubscription;
            }

            private IDisposable CreateExecutedSubscription(IObserver<TExecutableWork> observer)
            {
                if (IsWorkCompleted)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                workExecuted += observer.OnNext;
                workCompleted += observer.OnCompleted;
                if (ShouldForwardErrorToSubscribers())
                {
                    work.TriggerEvents.TriggerErrored += observer.OnError;
                }
                work.TriggerEvents.TriggerCompleted += observer.OnCompleted;

                return Disposable.Create(
                    () =>
                        {
                            workExecuted -= observer.OnNext;
                            workCompleted -= observer.OnCompleted;
                            work.TriggerEvents.TriggerCompleted -= observer.OnCompleted;
                            work.TriggerEvents.TriggerErrored -= observer.OnError;
                        });
            }

            private bool IsWorkCompleted
            {
                get
                {
                    return work.State == WorkState.Completed ||
                           work.State == WorkState.WorkerError ||
                           work.State == WorkState.TriggerError;
                }
            }

            private bool ShouldForwardErrorToSubscribers()
            {
                return work.Registration.WorkRegistry.DefaultOptions.TriggerErrorBehavior
                       == TriggerErrorBehavior.CompleteWorkAndThrow;
            }

            private void OnWorkExecuted(TExecutableWork triggeredWork)
            {
                var handler = workExecuted;
                if (handler != null)
                {
                    handler(triggeredWork);
                }
            }
        }
    }
}