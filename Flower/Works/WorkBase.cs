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
            TriggerEvents = new TriggerEvents();
        }

        public WorkState State { get; private set; }
        IWorkRegistrationBase IWorkBase.Registration { get { return Registration; } }
        public IWorkRegistrationBase<TInput> Registration { get; private set; }
        public ITriggerEvents TriggerEvents { get; private set; }

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
            var workRunner = ResolveWorkRunner();
            var triggeredWork = Trigger(workRunner, input);
            workRunner.Submit(triggeredWork);        
        }

        private IWorkRunner ResolveWorkRunner()
        {
            return Registration.Options.WorkRunnerResolver.Resolve(this);
        }

        private void TriggerOnError(Exception exception)
        {
            Registration.WorkRegistry.Unregister(this);
            State = WorkState.TriggerError;
            TriggerEvents.RaiseTriggerErrored(exception);
        }

        private void TriggerOnCompleted()
        {
            Registration.WorkRegistry.Unregister(this);
            State = WorkState.Completed;
            TriggerEvents.RaiseTriggerCompleted();
        }
    }
}