using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal abstract class ExecutableWork<TInput> : IExecutableWork<TInput>
    {
        private readonly IRegisteredWork<TInput> work; 
        public ExecutableWorkState State { get; private set; }
        public IWork<TInput> Work { get { return work; } }
        public TInput Input { get; private set; }
        IWork ITriggeredWork.Work { get { return Work; } }
        public IWorkRunner WorkRunner { get; private set; }
        public Exception Error { get; private set; }

        protected ExecutableWork(IWorkRunner workRunner, IRegisteredWork<TInput> work, TInput input)
        {
            WorkRunner = workRunner;
            this.work = work;
            Input = input;
            State = ExecutableWorkState.Pending;
        }

        public void Execute()
        {
            if (State != ExecutableWorkState.Pending)
            {
                throw new InvalidOperationException("Work can be executed only once.");
            }

            try
            {
                State = ExecutableWorkState.Executing;
                CreateWorkerScope();
                ExecuteWorker();
                DisposeWorkerScope();
                State = ExecutableWorkState.Success;
                WorkerExecuted();
            }
            catch (Exception e)
            {
                State = ExecutableWorkState.Error;
                Error = e;
                WorkerErrored(e);
            }
        }

        private void WorkerErrored(Exception error)
        {
            if (ShouldNotifyWorkerExecutedWhenWorkerErrored())
            {
                WorkerExecuted();
            }

            switch (work.Registration.WorkRegistry.DefaultOptions.WorkerErrorBehavior)
            {
                case WorkerErrorBehavior.SwallowErrorAndContinue:
                case WorkerErrorBehavior.RaiseExecutedAndContinue:
                    // Log.Warning("Continue on worker error: {0}.", error);
                    break;
                case WorkerErrorBehavior.RaiseExecutedAndCompleteWork:
                case WorkerErrorBehavior.SwallowErrorAndCompleteWork:
                    work.Complete(WorkState.WorkerError);
                    break;
                case WorkerErrorBehavior.CompleteWorkAndThrow:
                    work.Complete(WorkState.WorkerError);
                    throw error;
            }
        }

        private bool ShouldNotifyWorkerExecutedWhenWorkerErrored()
        {
            switch (State)
            {
                case ExecutableWorkState.Success:
                    return true;
                case ExecutableWorkState.Error:
                    return
                        work.Registration.Options.WorkerErrorBehavior == WorkerErrorBehavior.RaiseExecutedAndContinue ||
                        work.Registration.Options.WorkerErrorBehavior == WorkerErrorBehavior.RaiseExecutedAndCompleteWork;
                default:
                    return false;
            }
        }

        protected abstract void CreateWorkerScope();
        protected abstract void ExecuteWorker();
        protected abstract void DisposeWorkerScope();
        protected abstract void WorkerExecuted();
    }

    internal class ExecutableActionWork : ExecutableWork<object>, IExecutableActionWork
    {
        private readonly IRegisteredActionWork work;

        public ExecutableActionWork(IWorkRunner workRunner, IRegisteredActionWork work, object input)
            : base(workRunner, work, input)
        {
            this.work = work;
        }

        public IScope<IWorker> WorkerScope { get; private set; }

        protected override void CreateWorkerScope()
        {
            WorkerScope = work.Registration.CreateWorkerScope();
        }

        protected override void ExecuteWorker()
        {
            WorkerScope.Worker.Execute();
        }

        protected override void DisposeWorkerScope()
        {
            WorkerScope.Dispose();
        }

        protected override void WorkerExecuted()
        {
            work.WorkerExecuted(this);
        }
    }

    internal class ExecutableActionWork<TInput> : ExecutableWork<TInput>, IExecutableActionWork<TInput>
    {
        private readonly IRegisteredActionWork<TInput> work;

        public ExecutableActionWork(IWorkRunner workRunner, IRegisteredActionWork<TInput> work, TInput input)
            : base(workRunner, work, input)
        {
            this.work = work;
        }

        IWork ITriggeredWork.Work { get { return Work; } }
        IWork<TInput> ITriggeredWork<TInput>.Work { get { return Work; } }
        public new IActionWork<TInput> Work { get { return work; } }
        public IScope<IWorker<TInput>> WorkerScope { get; private set; }

        protected override void CreateWorkerScope()
        {
            WorkerScope = work.Registration.CreateWorkerScope();
        }

        protected override void ExecuteWorker()
        {
            WorkerScope.Worker.Execute(Input);
        }

        protected override void DisposeWorkerScope()
        {
            WorkerScope.Dispose();
        }

        protected override void WorkerExecuted()
        {
            work.WorkerExecuted(this);
        }
    }

    internal class ExecutableFuncWork<TInput, TOutput> : ExecutableWork<TInput>, IExecutableFuncWork<TInput, TOutput>
    {
        private readonly IRegisteredFuncWork<TInput, TOutput> work;

        public ExecutableFuncWork(IWorkRunner workRunner, IRegisteredFuncWork<TInput, TOutput> work, TInput input)
            : base(workRunner, work, input)
        {
            this.work = work;
        }

        IWork ITriggeredWork.Work { get { return Work; } }
        IWork<TInput> ITriggeredWork<TInput>.Work { get { return Work; } }
        public new IFuncWork<TInput, TOutput> Work { get { return work; } }
        public IScope<IWorker<TInput, TOutput>> WorkerScope { get; private set; }
        public TOutput Output { get; private set; }

        protected override void CreateWorkerScope()
        {
            WorkerScope = work.Registration.CreateWorkerScope();
        }

        protected override void ExecuteWorker()
        {
            Output = WorkerScope.Worker.Execute(Input);
        }

        protected override void DisposeWorkerScope()
        {
            WorkerScope.Dispose();
        }

        protected override void WorkerExecuted()
        {
            work.WorkerExecuted(this);
        }
    }
}