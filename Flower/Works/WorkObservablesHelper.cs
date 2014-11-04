using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Flower.Works
{
    internal class WorkObservablesHelper<TWork, TTriggeredWork>
        where TWork : IRegisteredWork where TTriggeredWork : ITriggeredWork
    {
        private readonly WorkTriggeredHelper workTriggeredHelper;
        private readonly WorkExecutedHelper workExecutedHelper;

        public WorkObservablesHelper(TWork work)
        {
            workTriggeredHelper = new WorkTriggeredHelper();
            workExecutedHelper = new WorkExecutedHelper(work);
        }

        public IObservable<TTriggeredWork> WorkTriggered { get { return workTriggeredHelper.WorkTriggered; } }
        public IObservable<TTriggeredWork> WorkExecuted { get { return workExecutedHelper.WorkExecuted; } }

        public void RaiseWorkTriggered(TTriggeredWork triggeredWork)
        {
            workTriggeredHelper.RaiseTriggered(triggeredWork);
        }

        public void RaiseWorkExecuted(TTriggeredWork triggeredWork)
        {
            workExecutedHelper.RaiseWorkExecuted(triggeredWork);
        }

        private class WorkTriggeredHelper
        {
            private event Action<TTriggeredWork> workTriggered;

            public WorkTriggeredHelper()
            {
                WorkTriggered = Observable.Create(CreateTriggeredSubscription());
            }

            public IObservable<TTriggeredWork> WorkTriggered { get; private set; }

            public void RaiseTriggered(TTriggeredWork triggeredWork)
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
                return Disposable.Create(() => { workTriggered -= observer.OnNext; });
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

        private class WorkExecutedHelper
        {
            private readonly TWork work;
            private event Action<TTriggeredWork> workExecuted;

            public WorkExecutedHelper(TWork work)
            {
                this.work = work;
                WorkExecuted = Observable.Create(CreateExecutedSubscription());
            }

            public IObservable<TTriggeredWork> WorkExecuted { get; private set; }

            public void RaiseWorkExecuted(TTriggeredWork triggeredWork)
            {
                OnWorkExecuted(triggeredWork);
            }

            private Func<IObserver<TTriggeredWork>, IDisposable> CreateExecutedSubscription()
            {
                return CreateExecutedSubscription;
            }

            private IDisposable CreateExecutedSubscription(IObserver<TTriggeredWork> observer)
            {
                if (IsWorkCompleted)
                {
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                workExecuted += observer.OnNext;
                if (ShouldForwardErrorToSubscribers())
                {
                    work.TriggerEvents.TriggerErrored += observer.OnError;
                }
                work.TriggerEvents.TriggerCompleted += observer.OnCompleted;

                return Disposable.Create(
                    () =>
                        {
                            workExecuted -= observer.OnNext;
                            work.TriggerEvents.TriggerCompleted -= observer.OnCompleted;
                            work.TriggerEvents.TriggerErrored -= observer.OnError;
                        });
            }

            private bool IsWorkCompleted
            {
                get
                {
                    return work.State == WorkState.Completed || work.State == WorkState.WorkerError
                           || work.State == WorkState.TriggerError;
                }
            }

            private bool ShouldForwardErrorToSubscribers()
            {
                return work.Registration.WorkRegistry.Options.TriggerErrorBehavior
                       == TriggerErrorBehavior.CompleteWorkAndForwardError;
            }

            private void OnWorkExecuted(TTriggeredWork triggeredWork)
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