using System;
using System.Linq;
using System.Reactive.Linq;
using Flower.WorkRunners;

namespace Flower.Works
{
    using Flower.Workers;

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
        public WorkerError LastError { get; private set; }
        IWorkRegistration IWork.Registration => Registration;

        public virtual IObservable<IExecutableWork> Executed
        {
            get
            {
                throw new InvalidOperationException("Only subclass implementations should be callable.");
            }
        }

        public virtual IObservable<IWork> Completed
        {
            get
            {
                throw new InvalidOperationException("Only subclass implementations should be callable.");
            }
        }

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

        public void SetWorkerError(WorkerError workerError)
        {
            LastError = workerError;
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

        public new IActionWorkRegistration Registration { get; }
        public IObservable<ITriggeredActionWork> Triggered => observables.WorkTriggered;
        IObservable<IExecutableWork> IWork.Executed => Executed.Select(executedWork => (IExecutableWork)executedWork);
        public new IObservable<IExecutableActionWork> Executed => observables.WorkExecuted;
        IObservable<IWork> IWork.Completed => Completed.Select(workCompleted => (IWork)workCompleted);
        public new IObservable<IActionWork> Completed => observables.WorkCompleted;

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
            observables.RaiseWorkCompleted(this);
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

        public new IActionWorkRegistration<TInput> Registration { get; }
        public IObservable<ITriggeredActionWork<TInput>> Triggered => observables.WorkTriggered;
        IObservable<IExecutableWork> IWork.Executed => Executed.Select(executedWork => (IExecutableWork)executedWork);
        public new IObservable<IExecutableActionWork<TInput>> Executed => observables.WorkExecuted;
        IObservable<IWork> IWork.Completed => Completed.Select(workCompleted => (IWork)workCompleted);
        public new IObservable<IActionWork<TInput>> Completed => observables.WorkCompleted;

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
            observables.RaiseWorkCompleted(this);
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

        public new IFuncWorkRegistration<TInput, TOutput> Registration { get; }
        public IObservable<ITriggeredFuncWork<TInput, TOutput>> Triggered => observables.WorkTriggered;
        IObservable<IExecutableWork> IWork.Executed => Executed.Select(workCompleted => (IExecutableWork)workCompleted);
        public new IObservable<IExecutableFuncWork<TInput, TOutput>> Executed => observables.WorkExecuted;
        IObservable<IWork> IWork.Completed => Completed.Select(workCompleted => (IWork)workCompleted);
        public new IObservable<IFuncWork<TInput, TOutput>> Completed => observables.WorkCompleted;
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
            observables.RaiseWorkCompleted(this);
        }
    
        private static bool WorkSucceeded(IExecutableFuncWork<TInput, TOutput> executedWork)
        {
            return executedWork.State == ExecutableWorkState.Success;
        }
    }
}