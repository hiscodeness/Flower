using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Flower.Works
{
    internal class WorkObservables<TWork, TTriggeredWork> where TWork : IRegisteredWorkBase where TTriggeredWork : ITriggeredWorkBase
    {
        private readonly TWork work;
        private event Action<TTriggeredWork> WorkTriggered;
        private event Action<TTriggeredWork> WorkExecuted;

        internal WorkObservables(TWork work)
        {
            this.work = work;
            Triggered = Observable.Create(CreateTriggeredSubscription());
            Executed = Observable.Create(CreateExecutedSubscription());
        }

        internal IObservable<TTriggeredWork> Triggered { get; private set; }
        internal IObservable<TTriggeredWork> Executed { get; private set; }

        internal void TriggeredWorkCreated(TTriggeredWork triggeredWork)
        {
            OnTriggeredWorkCreated(triggeredWork);
        }

        internal void TriggeredWorkExecuted(TTriggeredWork triggeredWork)
        {
            OnWorkExecuted(triggeredWork);
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

            return Disposable.Create(() =>
            {
                WorkExecuted -= observer.OnNext;
                work.TriggerEvents.TriggerCompleted -= observer.OnCompleted;
                work.TriggerEvents.TriggerErrored -= observer.OnError;
            });
        }

        private bool IsWorkCompleted { get
        {
            return work.State == WorkState.Completed ||
                   work.State == WorkState.WorkerError ||
                   work.State == WorkState.TriggerError;
        }
        }

        private bool ShouldForwardErrorToSubscribers()
        {
            return work.Registration.WorkRegistry.Options.TriggerErrorBehavior ==
                   TriggerErrorBehavior.CompleteWorkAndForwardError;
        }

        private void OnTriggeredWorkCreated(TTriggeredWork triggeredWork)
        {
            var handler = WorkTriggered;
            if (handler != null)
            {
                handler(triggeredWork);
            }
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