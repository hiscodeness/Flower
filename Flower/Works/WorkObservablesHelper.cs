using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Flower.Works
{
    internal class WorkObservablesHelper<TWork, TTriggeredWork>
        where TWork : IRegisteredWork where TTriggeredWork : ITriggeredWork
    {
        private readonly WorkTriggeringListener workTriggeringListener;
        private readonly WorkExecutionListener workExecutionListener;

        public WorkObservablesHelper(TWork work)
        {
            workTriggeringListener = new WorkTriggeringListener();
            workExecutionListener = new WorkExecutionListener(work);
        }

        public IObservable<TTriggeredWork> Triggered { get { return workTriggeringListener.Triggered; } }
        public IObservable<TTriggeredWork> Executed { get { return workExecutionListener.Executed; } }

        public void RaiseTriggered(TTriggeredWork triggeredWork)
        {
            workTriggeringListener.TriggeredWorkCreated(triggeredWork);
        }

        public void RaiseExecuted(TTriggeredWork triggeredWork)
        {
            workExecutionListener.TriggeredWorkExecuted(triggeredWork);
        }

        private class WorkTriggeringListener
        {
            private event Action<TTriggeredWork> WorkTriggered;

            internal WorkTriggeringListener()
            {
                Triggered = Observable.Create(CreateTriggeredSubscription());
            }

            internal IObservable<TTriggeredWork> Triggered { get; private set; }

            internal void TriggeredWorkCreated(TTriggeredWork triggeredWork)
            {
                OnTriggeredWorkCreated(triggeredWork);
            }

            private Func<IObserver<TTriggeredWork>, IDisposable> CreateTriggeredSubscription()
            {
                return CreateTriggeredSubscription;
            }

            private IDisposable CreateTriggeredSubscription(IObserver<TTriggeredWork> observer)
            {
                WorkTriggered += observer.OnNext;
                return Disposable.Create(() => { WorkTriggered -= observer.OnNext; });
            }

            private void OnTriggeredWorkCreated(TTriggeredWork triggeredWork)
            {
                var handler = WorkTriggered;
                if (handler != null)
                {
                    handler(triggeredWork);
                }
            }
        }

        private class WorkExecutionListener
        {
            private readonly TWork work;
            private event Action<TTriggeredWork> WorkExecuted;

            internal WorkExecutionListener(TWork work)
            {
                this.work = work;
                Executed = Observable.Create(CreateExecutedSubscription());
            }

            internal IObservable<TTriggeredWork> Executed { get; private set; }

            internal void TriggeredWorkExecuted(TTriggeredWork triggeredWork)
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

                WorkExecuted += observer.OnNext;
                if (ShouldForwardErrorToSubscribers())
                {
                    work.TriggerEvents.TriggerErrored += observer.OnError;
                }
                work.TriggerEvents.TriggerCompleted += observer.OnCompleted;

                return Disposable.Create(
                    () =>
                        {
                            WorkExecuted -= observer.OnNext;
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
                var handler = WorkExecuted;
                if (handler != null)
                {
                    handler(triggeredWork);
                }
            }
        }
    }
}