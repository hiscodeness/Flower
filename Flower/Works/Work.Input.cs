using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class Work<TInput> : IWork<TInput>, IRegisteredWork<TInput>
    {
        private IDisposable triggerSubscription;

        public Work(IWorkRegistration<TInput> registration)
        {
            State = WorkState.Suspended;
            Registration = registration;
            Observables = new WorkObservables<IWork<TInput>, ITriggeredWork<TInput>>(this);
        }

        public WorkState State { get; private set; }
        IWorkRegistrationBase IWorkBase.Registration { get { return Registration; } }
        IWorkRegistrationBase<TInput> IWorkBase<TInput>.Registration { get { return Registration; } }
        public IWorkRegistration<TInput> Registration { get; private set; }
        public IObservable<ITriggeredWork<TInput>> Triggered { get { return Observables.Triggered; } }
        public IObservable<ITriggeredWork<TInput>> Executed { get { return Observables.Executed; } }
        internal WorkObservables<IWork<TInput>, ITriggeredWork<TInput>> Observables { get; private set; }

        public ITriggeredWorkBase Trigger(IWorkRunner workRunner, TInput input)
        {
            var triggeredWork = new TriggeredWork<TInput>(workRunner, this, input);
            Observables.TriggeredWorkCreated(triggeredWork);
            return triggeredWork;
        }

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

        private void TriggerOnNext(TInput input)
        {
            ((WorkRegistry)Registration.WorkRegistry).Triggered(this, input);
        }

        private void TriggerOnError(Exception exception)
        {
            ((WorkRegistry)Registration.WorkRegistry).TriggerErrored(this, exception);
            State = WorkState.TriggerError;
            Observables.OnTriggerErrored(exception);
        }

        private void TriggerOnCompleted()
        {
            ((WorkRegistry)Registration.WorkRegistry).TriggerCompleted(this);
            State = WorkState.Completed;
            Observables.OnWorkCompleted();
        }
    }
}