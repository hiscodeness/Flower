using System;
using System.Linq;
using System.Reactive.Linq;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal abstract class Work<TInput> : IRegisteredWork<TInput>
    {
        private IDisposable triggerSubscription;
        private bool isBeingRemovedFromWorkRegistry;

        protected Work(IWorkRegistration<TInput> registration)
        {
            Registration = registration;
            TriggerEvents = new TriggerEvents();
        }

        public WorkState State { get; private set; }
        IWorkRegistration IWork.Registration => Registration;
        public IWorkRegistration<TInput> Registration { get; }
        public ITriggerEvents TriggerEvents { get; }

        public void Activate()
        {
            if (State == WorkState.Active) return;

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

        public void Complete(WorkState withState)
        {
            if (Registration.WorkRegistry.Works.Contains(this))
            {
                isBeingRemovedFromWorkRegistry = true;
                Registration.WorkRegistry.Complete(this);
                isBeingRemovedFromWorkRegistry = false;
            }

            if (isBeingRemovedFromWorkRegistry) return;

            State = withState;
            WorkCompleted();
        }

        void IWork.Complete()
        {
            Complete(WorkState.Completed);
        }

        protected abstract void WorkTriggered(ITriggeredWork triggeredWork);
        protected abstract IExecutableWork CreateExecutableWork(IWorkRunner workRunner, TInput input);
        protected abstract void WorkCompleted();

        private void TriggerOnNext(TInput input)
        {
            var workRunner = Registration.Options.WorkRunnerFactory(this);
            var triggeredWork = CreateExecutableWork(workRunner, input);
            WorkTriggered(triggeredWork);
            try
            {
                workRunner.Submit(triggeredWork).Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count == 1)
                {
                    throw ex.InnerExceptions[0];
                }
                throw;
            }
        }

        private void TriggerOnError(Exception exception)
        {
            TriggerEvents.RaiseTriggerErrored(exception);
            Complete(WorkState.TriggerError);
        }

        private void TriggerOnCompleted()
        {
            TriggerEvents.RaiseTriggerCompleted();
            Complete(WorkState.Completed);
        }
    }

    internal class ActionWork : Work<object>, IRegisteredActionWork
    {
        private readonly ActionWorkObservablesHelper observables;

        public ActionWork(IActionWorkRegistration registration)
            : base(registration)
        {
            Registration = registration;
            observables = new ActionWorkObservablesHelper(this);
        }

        new public IActionWorkRegistration Registration { get; }
        public IObservable<ITriggeredActionWork> Triggered => observables.WorkTriggered;
        public IObservable<IExecutableActionWork> Executed => observables.WorkExecuted;

        protected override IExecutableWork CreateExecutableWork(IWorkRunner workRunner, object input)
        {
            var executableWork = new ExecutableActionWork(workRunner, this, input);
            return Registration.Options.IsWorkDecorated
                ? Registration.Options.WorkDecoratorFactory.Decorate(executableWork)
                : executableWork;
        }

        protected override void WorkTriggered(ITriggeredWork triggeredWork)
        {
            observables.RaiseWorkTriggered(triggeredWork as ITriggeredActionWork);
        }

        public void WorkerExecuted(IExecutableActionWork triggeredWork)
        {
            observables.RaiseWorkExecuted(triggeredWork);
        }

        protected override void WorkCompleted()
        {
            observables.RaiseWorkCompleted();
        }
    }

    internal class ActionWork<TInput> : Work<TInput>, IRegisteredActionWork<TInput>
    {
        private readonly ActionWorkObservablesHelper<TInput> observables;

        public ActionWork(IActionWorkRegistration<TInput> registration)
            : base(registration)
        {
            Registration = registration;
            observables = new ActionWorkObservablesHelper<TInput>(this);
        }

        new public IActionWorkRegistration<TInput> Registration { get; }
        public IObservable<ITriggeredActionWork<TInput>> Triggered => observables.WorkTriggered;
        public IObservable<IExecutableActionWork<TInput>> Executed => observables.WorkExecuted;

        protected override IExecutableWork CreateExecutableWork(IWorkRunner workRunner, TInput input)
        {
            var executableWork = new ExecutableActionWork<TInput>(workRunner, this, input);
            return Registration.Options.IsWorkDecorated
                ? Registration.Options.WorkDecoratorFactory.Decorate(executableWork)
                : executableWork;
        }

        protected override void WorkTriggered(ITriggeredWork triggeredWork)
        {
            observables.RaiseWorkTriggered(triggeredWork as ITriggeredActionWork<TInput>);
        }

        public void WorkerExecuted(IExecutableActionWork<TInput> triggeredWork)
        {
            observables.RaiseWorkExecuted(triggeredWork);
        }
        
        protected override void WorkCompleted()
        {
            observables.RaiseWorkCompleted();
        }
    }

    internal class FuncWork<TInput, TOutput> : Work<TInput>, IRegisteredFuncWork<TInput, TOutput>
    {
        private readonly FuncWorkObservablesHelper<TInput, TOutput> observables;

        public FuncWork(IFuncWorkRegistration<TInput, TOutput> registration)
            : base(registration)
        {
            Registration = registration;
            observables = new FuncWorkObservablesHelper<TInput, TOutput>(this);
            Output = Executed.Where(WorkSucceeded).Select(executedWork => executedWork.Output);
        }        

        new public IFuncWorkRegistration<TInput, TOutput> Registration { get; }
        public IObservable<ITriggeredFuncWork<TInput, TOutput>> Triggered => observables.WorkTriggered;
        public IObservable<IExecutableFuncWork<TInput, TOutput>> Executed => observables.WorkExecuted;
        public IObservable<TOutput> Output { get; }

        protected override IExecutableWork CreateExecutableWork(IWorkRunner workRunner, TInput input)
        {
            var executableWork = new ExecutableFuncWork<TInput, TOutput>(workRunner, this, input);
            return Registration.Options.IsWorkDecorated
                ? Registration.Options.WorkDecoratorFactory.Decorate(executableWork)
                : executableWork;
        }

        protected override void WorkTriggered(ITriggeredWork triggeredWork)
        {
            observables.RaiseWorkTriggered(triggeredWork as ITriggeredFuncWork<TInput, TOutput>);
        }

        public void WorkerExecuted(IExecutableFuncWork<TInput, TOutput> triggeredWork)
        {
            observables.RaiseWorkExecuted(triggeredWork);
        }

        protected override void WorkCompleted()
        {
            observables.RaiseWorkCompleted();
        }
    
        private static bool WorkSucceeded(IExecutableFuncWork<TInput, TOutput> executedWork)
        {
            return executedWork.State == ExecutableWorkState.Success;
        }
    }
}