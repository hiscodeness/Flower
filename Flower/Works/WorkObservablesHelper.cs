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
        private readonly WorkCompletedHelper workCompletedHelper;
        private readonly WorkTriggeredHelper workTriggeredHelper;
        private readonly WorkExecutedHelper workExecutedHelper;

        protected WorkObservablesHelper(TWork work)
        {
            workCompletedHelper = new WorkCompletedHelper();
            workTriggeredHelper = new WorkTriggeredHelper();
            workExecutedHelper = new WorkExecutedHelper(work);
        }

        public IObservable<TTriggeredWork> WorkTriggered => workTriggeredHelper.WorkTriggered;
        public IObservable<TExecutableWork> WorkExecuted => workExecutedHelper.WorkExecuted;
        public IObservable<TWork> WorkCompleted => workCompletedHelper.WorkCompleted;

        public void RaiseWorkTriggered(TTriggeredWork triggeredWork)
        {
            workTriggeredHelper.RaiseWorkTriggered(triggeredWork);
        }

        public void RaiseWorkExecuted(TExecutableWork triggeredWork)
        {
            workExecutedHelper.RaiseWorkExecuted(triggeredWork);
        }

        public void RaiseWorkCompleted(TWork work)
        {
            workCompletedHelper.RaiseWorkCompleted(work);
            workTriggeredHelper.RaiseWorkCompleted(work);
            workExecutedHelper.RaiseWorkCompleted(work);
        }

        private class WorkCompletedHelper
        {
            protected event Action workCompleted;
            private event Action<TWork> typedWorkCompleted;

            public WorkCompletedHelper()
            {
                WorkCompleted = Observable.Create(CreateCompletedSubscription());
            }

            public IObservable<TWork> WorkCompleted { get; }

            public void RaiseWorkCompleted(TWork work)
            {
                OnWorkCompleted(work);
            }

            private Func<IObserver<TWork>, IDisposable> CreateCompletedSubscription()
            {
                return CreateCompletedSubscription;
            }

            private IDisposable CreateCompletedSubscription(IObserver<TWork> observer)
            {
                typedWorkCompleted += observer.OnNext;
                return Disposable.Create(() =>
                {
                    typedWorkCompleted -= observer.OnNext;
                });
            }

            private void OnWorkCompleted(TWork work)
            {
                workCompleted?.Invoke();
                typedWorkCompleted?.Invoke(work);
            }
        }

        private class WorkTriggeredHelper : WorkCompletedHelper
        {
            private event Action<TTriggeredWork> workTriggered;

            public WorkTriggeredHelper()
            {
                WorkTriggered = Observable.Create(CreateTriggeredSubscription());
            }

            public IObservable<TTriggeredWork> WorkTriggered { get; }

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
                handler?.Invoke(triggeredWork);
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

            public IObservable<TExecutableWork> WorkExecuted { get; }

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
                handler?.Invoke(triggeredWork);
            }
        }
    }
}