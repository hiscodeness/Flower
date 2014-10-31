using System;
using System.Linq;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal abstract class WorkBase<TInput> : IRegisteredWorkBase<TInput>, IWorkBase<TInput>
    {
        private IDisposable triggerSubscription;

        protected WorkBase(IWorkRegistrationBase<TInput> registration)
        {
            Registration = registration;
        }

        public WorkState State { get; private set; }
        IWorkRegistrationBase IWorkBase.Registration { get { return Registration; } }
        public IWorkRegistrationBase<TInput> Registration { get; private set; }

        public ITriggeredWorkBase Trigger(IWorkRunner workRunner, TInput input)
        {
            var triggeredWork = CreateTriggeredWork(workRunner, input);
            TriggeredWorkCreated(triggeredWork);
            return triggeredWork;
        }

        public void Activate()
        {
            if (State != WorkState.Suspended)
            {
                throw new InvalidOperationException("Only suspended work can be activated.");
            }

            State = WorkState.Active;
            triggerSubscription = Registration.Trigger.Subscribe(TriggerOnNext, TriggerOnError, TriggerOnCompleted);
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

        protected abstract void TriggeredWorkCreated(ITriggeredWorkBase triggeredWork);
        protected abstract ITriggeredWorkBase CreateTriggeredWork(IWorkRunner workRunner, TInput input);
        protected abstract void TriggerErrored(Exception exception);
        protected abstract void TriggerCompleted();
        
        protected void WorkerErrored(Exception error)
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
            var workRunner = Registration.Options.WorkRunnerResolver.Resolve(this);
            var triggeredWork = Trigger(workRunner, input);
            triggeredWork.Submit();        
        }

        private void TriggerOnError(Exception exception)
        {
            Registration.WorkRegistry.Unregister(this);
            State = WorkState.TriggerError;
            TriggerErrored(exception);
        }

        private void TriggerOnCompleted()
        {
            Registration.WorkRegistry.Unregister(this);
            State = WorkState.Completed;
            TriggerCompleted();
        }
    }
}