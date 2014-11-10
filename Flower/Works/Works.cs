using System;
using System.Linq;
using System.Reactive.Linq;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal abstract class Work<TInput> : IRegisteredWork<TInput>
    {
        private IDisposable triggerSubscription;

        protected Work(IWorkRegistration<TInput> registration)
        {
            Registration = registration;
            TriggerEvents = new TriggerEvents();
        }

        public WorkState State { get; private set; }
        IWorkRegistration IWork.Registration { get { return Registration; } }
        public IWorkRegistration<TInput> Registration { get; private set; }
        public ITriggerEvents TriggerEvents { get; private set; }

        public IExecutableWork Trigger(IWorkRunner workRunner, TInput input)
        {
            var triggeredWork = CreateExecutableWork(workRunner, input);
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

        protected abstract void TriggeredWorkCreated(ITriggeredWork triggeredWork);
        protected abstract IExecutableWork CreateExecutableWork(IWorkRunner workRunner, TInput input);

        public void WorkerErrored(Exception error)
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

    internal class ActionWork : Work<object>, IRegisteredActionWork
    {
        private readonly WorkObservablesHelper<IRegisteredActionWork, ITriggeredActionWork, IExecutableActionWork> observables;

        public ActionWork(IActionWorkRegistration registration)
            : base(registration)
        {
            Registration = registration;
            observables = new WorkObservablesHelper<IRegisteredActionWork, ITriggeredActionWork, IExecutableActionWork>(this);
        }

        new public IActionWorkRegistration Registration { get; private set; }
        public IObservable<ITriggeredActionWork> Triggered { get { return observables.WorkTriggered; } }
        public IObservable<IExecutableActionWork> Executed { get { return observables.WorkExecuted; } }

        protected override IExecutableWork CreateExecutableWork(IWorkRunner workRunner, object input)
        {
            return new ExecutableActionWork(workRunner, this, input);
        }

        protected override void TriggeredWorkCreated(ITriggeredWork triggeredWork)
        {
            observables.RaiseWorkTriggered(triggeredWork as ITriggeredActionWork);
        }

        public void WorkerExecuted(IExecutableActionWork triggeredWork)
        {
            observables.RaiseWorkExecuted(triggeredWork);
        }
    }

    internal class ActionWork<TInput> : Work<TInput>, IRegisteredActionWork<TInput>
    {
        private readonly WorkObservablesHelper<IRegisteredActionWork<TInput>, ITriggeredActionWork<TInput>, IExecutableActionWork<TInput>> observables;

        public ActionWork(IActionWorkRegistration<TInput> registration)
            : base(registration)
        {
            Registration = registration;
            observables = new WorkObservablesHelper<IRegisteredActionWork<TInput>, ITriggeredActionWork<TInput>, IExecutableActionWork<TInput>>(this);
        }

        new public IActionWorkRegistration<TInput> Registration { get; private set; }
        public IObservable<ITriggeredActionWork<TInput>> Triggered { get { return observables.WorkTriggered; } }
        public IObservable<IExecutableActionWork<TInput>> Executed { get { return observables.WorkExecuted; } }

        protected override IExecutableWork CreateExecutableWork(IWorkRunner workRunner, TInput input)
        {
            return new ExecutableActionWork<TInput>(workRunner, this, input);
        }

        protected override void TriggeredWorkCreated(ITriggeredWork triggeredWork)
        {
            observables.RaiseWorkTriggered(triggeredWork as ITriggeredActionWork<TInput>);
        }

        public void WorkerExecuted(IExecutableActionWork<TInput> triggeredWork)
        {
            observables.RaiseWorkExecuted(triggeredWork);
        }
    }

    internal class FuncWork<TInput, TOutput> : Work<TInput>, IRegisteredFuncWork<TInput, TOutput>
    {
        private readonly WorkObservablesHelper<IRegisteredFuncWork<TInput, TOutput>, ITriggeredFuncWork<TInput, TOutput>, IExecutableFuncWork<TInput, TOutput>> observables;

        public FuncWork(IFuncWorkRegistration<TInput, TOutput> registration)
            : base(registration)
        {
            Registration = registration;
            observables = new WorkObservablesHelper<IRegisteredFuncWork<TInput, TOutput>, ITriggeredFuncWork<TInput, TOutput>, IExecutableFuncWork<TInput, TOutput>>(this);
            Output = Executed.Select(executedWork => executedWork.Output);
        }

        new public IFuncWorkRegistration<TInput, TOutput> Registration { get; private set; }
        public IObservable<ITriggeredFuncWork<TInput, TOutput>> Triggered { get { return observables.WorkTriggered; } }
        public IObservable<IExecutableFuncWork<TInput, TOutput>> Executed { get { return observables.WorkExecuted; } }
        public IObservable<TOutput> Output { get; private set; }

        protected override IExecutableWork CreateExecutableWork(IWorkRunner workRunner, TInput input)
        {
            return new ExecutableFuncWork<TInput, TOutput>(workRunner, this, input);
        }

        protected override void TriggeredWorkCreated(ITriggeredWork triggeredWork)
        {
            observables.RaiseWorkTriggered(triggeredWork as ITriggeredFuncWork<TInput, TOutput>);
        }

        public void WorkerExecuted(IExecutableFuncWork<TInput, TOutput> triggeredWork)
        {
            observables.RaiseWorkExecuted(triggeredWork);
        }
    }
}