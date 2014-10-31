using System;
using System.Linq;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal abstract class WorkBase<TInput> : IRegisteredWork<TInput>
    {
        private IDisposable triggerSubscription;

        protected WorkBase(IWorkRegistrationBase<TInput> registration)
        {
            this.Registration = registration;
        }

        public WorkState State { get; private set; }
        IWorkRegistrationBase IWorkBase.Registration { get { return this.Registration; } }
        public IWorkRegistrationBase<TInput> Registration { get; private set; }

        public ITriggeredWorkBase Trigger(IWorkRunner workRunner, TInput input)
        {
            var triggeredWork = CreateTriggeredWork(workRunner, input);
            TriggeredWorkCreated(triggeredWork);
            return triggeredWork;
        }

        public void Activate()
        {
            if (this.State != WorkState.Suspended)
            {
                throw new InvalidOperationException("Only suspended work can be activated.");
            }

            State = WorkState.Active;
            triggerSubscription = this.Registration.Trigger.Subscribe(TriggerOnNext, TriggerOnError, TriggerOnCompleted);
        }

        public void Suspend()
        {
            if (this.triggerSubscription == null) return;

            this.State = WorkState.Suspended;
            this.triggerSubscription.Dispose();
            this.triggerSubscription = null;
        }

        public void Unregister()
        {
            if (this.Registration.WorkRegistry.Works.Contains(this))
            {
                this.Registration.WorkRegistry.Unregister(this);
            }

            this.State = WorkState.Unregistered;
        }

        protected abstract void TriggeredWorkCreated(ITriggeredWorkBase triggeredWork);
        protected abstract ITriggeredWorkBase CreateTriggeredWork(IWorkRunner workRunner, TInput input);
        protected abstract void TriggerErrored(Exception exception);
        protected abstract void TriggerCompleted();
        
        internal void WorkerErrored(ITriggeredWorkBase triggeredWork, Exception error)
        {
            switch (this.Registration.WorkRegistry.Options.WorkerErrorBehavior)
            {
                case WorkerErrorBehavior.Ignore:
                    // Eats exception
                    //Log.Warning("Continue on worker error: {0}.", error);
                    break;
                case WorkerErrorBehavior.CompleteWork:
                    this.Registration.WorkRegistry.Unregister(this);
                    this.State = WorkState.WorkerError;
                    break;
                case WorkerErrorBehavior.CompleteWorkAndThrow:
                    this.Registration.WorkRegistry.Unregister(this);
                    this.State = WorkState.WorkerError;
                    throw error;
            }
        }

        private void TriggerOnNext(TInput input)
        {
            ((WorkRegistry)this.Registration.WorkRegistry).Triggered(this, input);
        }

        private void TriggerOnError(Exception exception)
        {
            ((WorkRegistry)this.Registration.WorkRegistry).TriggerErrored(this, exception);
            this.State = WorkState.TriggerError;
            this.TriggerErrored(exception);
        }

        private void TriggerOnCompleted()
        {
            ((WorkRegistry)this.Registration.WorkRegistry).TriggerCompleted(this);
            this.State = WorkState.Completed;
            this.TriggerCompleted();
        }
    }
}