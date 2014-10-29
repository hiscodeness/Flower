using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Flower.Works
{
    internal class Work<TInput> : IWork<TInput>
    {
        private IDisposable triggerSubscription;

        private event Action<ITriggeredWork<TInput>> WorkTriggered;
        private event Action<ITriggeredWork<TInput>> WorkExecuted;
        private event Action TriggerCompleted;
        private event Action<Exception> TriggerErrored;

        public Work(IWorkRegistration<TInput> registration)
        {
            Registration = registration;
            Triggered = Observable.Create(CreateTriggeredSubscription());
            Executed = Observable.Create(CreateExecutedSubscription());
        }

        public WorkState State { get; private set; }
        public IWorkRegistration<TInput> Registration { get; private set; }
        public IObservable<ITriggeredWork<TInput>> Triggered { get; private set; }
        public IObservable<ITriggeredWork<TInput>> Executed { get; private set; }

        public void Activate()
        {
            if (State != WorkState.Suspended)
            {
                throw new InvalidOperationException("Only suspended work can be activated.");
            }

            State = WorkState.Active;
            triggerSubscription = Registration.Trigger.Subscribe(TriggerOnNext,
                TriggerOnError,
                TriggerOnCompleted);
        }

        public void Suspend()
        {
            if (triggerSubscription == null) return;

            State = WorkState.Suspended;
            triggerSubscription.Dispose();
            triggerSubscription = null;
        }

        public void Unregister()
        {
            if (Registration.WorkRegistry.Works.Contains(this))
            {
                Registration.WorkRegistry.Unregister(this);
            }

            State = WorkState.Unregistered;
        }

        internal void TriggeredWorkCreated(ITriggeredWork<TInput> triggeredWork)
        {
            OnTriggeredWorkCreated(triggeredWork);
        }

        internal void TriggeredWorkExecuted(ITriggeredWork<TInput> triggeredWork)
        {
            OnWorkExecuted(triggeredWork);
        }

        internal void TriggeredWorkErrored(
            ITriggeredWork<TInput> triggeredWork, Exception error)
        {
            switch (Registration.WorkRegistry.Options.WorkerErrorBehavior)
            {
                case WorkerErrorBehavior.Ignore:
                    // Eats exception
                    //Log.Warning("Continue on worker error: {0}.", error);
                    break;
                case WorkerErrorBehavior.CompleteWork:
                    Registration.WorkRegistry.Unregister(this);
                    State = WorkState.WorkerError;
                    break;
                case WorkerErrorBehavior.CompleteWorkAndThrow:
                    Registration.WorkRegistry.Unregister(this);
                    State = WorkState.WorkerError;
                    throw error;
            }
        }

        private Func<IObserver<ITriggeredWork<TInput>>, IDisposable> CreateTriggeredSubscription()
        {
            return CreateTriggeredSubscription;
        }

        private IDisposable CreateTriggeredSubscription(IObserver<ITriggeredWork<TInput>> observer)
        {
            WorkTriggered += observer.OnNext;
            return Disposable.Create(() => { WorkTriggered -= observer.OnNext; });
        }

        private Func<IObserver<ITriggeredWork<TInput>>, IDisposable> CreateExecutedSubscription()
        {
            return CreateExecutedSubscription;
        }

        private IDisposable CreateExecutedSubscription(IObserver<ITriggeredWork<TInput>> observer)
        {
            if (State == WorkState.Completed ||
               State == WorkState.WorkerError ||
               State == WorkState.TriggerError)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            WorkExecuted += observer.OnNext;
            if (ShouldForwardErrorToSubscribers())
            {
                TriggerErrored += observer.OnError;
            }
            TriggerCompleted += observer.OnCompleted;

            return Disposable.Create(() =>
            {
                WorkExecuted -= observer.OnNext;
                TriggerCompleted -= observer.OnCompleted;
                TriggerErrored -= observer.OnError;
            });
        }

        private bool ShouldForwardErrorToSubscribers()
        {
            return Registration.WorkRegistry.Options.TriggerErrorBehavior == TriggerErrorBehavior.CompleteWorkAndForwardError;
        }

        private void OnTriggeredWorkCreated(ITriggeredWork<TInput> triggeredWork)
        {
            var handler = WorkTriggered;
            if (handler != null)
            {
                handler(triggeredWork);
            }
        }

        private void OnWorkExecuted(ITriggeredWork<TInput> triggeredWork)
        {
            var handler = WorkExecuted;
            if (handler != null)
            {
                handler(triggeredWork);
            }
        }

        private void OnTriggerErrored(Exception exception)
        {
            var handler = TriggerErrored;
            if (handler != null)
            {
                handler(exception);
            }
        }

        private void OnWorkCompleted()
        {
            var handler = TriggerCompleted;
            if (handler != null)
            {
                handler();
            }
        }

        private void TriggerOnNext(TInput input)
        {
            ((WorkRegistry)Registration.WorkRegistry).Triggered(this, input);
        }

        private void TriggerOnError(Exception exception)
        {
            ((WorkRegistry)Registration.WorkRegistry).TriggerErrored(this, exception);
            State = WorkState.TriggerError;
            OnTriggerErrored(exception);
        }

        private void TriggerOnCompleted()
        {
            ((WorkRegistry)Registration.WorkRegistry).TriggerCompleted(this);
            State = WorkState.Completed;
            OnWorkCompleted();
        }
    }
}