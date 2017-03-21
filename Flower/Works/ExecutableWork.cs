namespace Flower.Works
{
    using System;
    using System.Threading.Tasks;
    using Flower.WorkRunners;

    internal abstract class ExecutableWork<TInput> : IExecutableWork<TInput>
    {
        private readonly IRegisteredWork<TInput> work;
        public ExecutableWorkState State { get; private set; }
        public IWork<TInput> Work => work;
        public TInput Input { get; }
        IWork ITriggeredWork.Work => Work;
        public IWorkRunner WorkRunner { get; }
        public Exception Error { get; private set; }
        public IScope<object> WorkerScope => GetWorkerScope();

        protected ExecutableWork(IWorkRunner workRunner, IRegisteredWork<TInput> work, TInput input)
        {
            WorkRunner = workRunner;
            this.work = work;
            Input = input;
            State = ExecutableWorkState.Pending;
        }

        public async Task Execute()
        {
            if (State != ExecutableWorkState.Pending)
            {
                throw new InvalidOperationException("Work can be executed only once.");
            }

            try
            {
                State = ExecutableWorkState.Executing;
                CreateWorkerScope();
                await ExecuteWorker();
                DisposeWorkerScope();
                State = ExecutableWorkState.Success;
                OnWorkerExecuted();
            }
            catch (Exception e)
            {
                State = ExecutableWorkState.Error;
                WorkerErrored(e);
            }
        }

        private void WorkerErrored(Exception error)
        {
            Error = error;
            OnWorkerErrored(error);

            switch (work.Registration.WorkRegistry.Options.WorkerErrorMode)
            {
                case WorkerErrorMode.Continue:
                    // Log.Warning("Continue on worker error: {0}.", error);
                    break;
                case WorkerErrorMode.CompleteWork:
                    work.Complete(WorkState.WorkerError);
                    break;
                case WorkerErrorMode.CompleteWorkAndThrow:
                    work.Complete(WorkState.WorkerError);
                    throw error;
            }
        }

        protected abstract void CreateWorkerScope();
        protected abstract IScope<object> GetWorkerScope();
        protected abstract Task ExecuteWorker();
        protected abstract void DisposeWorkerScope();
        protected abstract void OnWorkerExecuted();
        protected abstract void OnWorkerErrored(Exception error);
    }

    internal class ExecutableActionWork : ExecutableWork<object>, IExecutableActionWork
    {
        private readonly IRegisteredActionWork work;

        public ExecutableActionWork(IWorkRunner workRunner, IRegisteredActionWork work, object input)
            : base(workRunner, work, input)
        {
            this.work = work;
        }

        public new IScope<IWorker> WorkerScope { get; private set; }

        protected override void CreateWorkerScope()
        {
            WorkerScope = work.Registration.CreateWorkerScope();
        }

        protected override IScope<object> GetWorkerScope()
        {
            return WorkerScope;
        }

        protected override async Task ExecuteWorker()
        {
            await WorkerScope.Worker.Execute();
        }

        protected override void DisposeWorkerScope()
        {
            WorkerScope.Dispose();
        }

        protected override void OnWorkerExecuted()
        {
            work.WorkerExecuted(this);
        }

        protected override void OnWorkerErrored(Exception error)
        {
            work.WorkerErrored(this, error);
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

        IWork ITriggeredWork.Work => Work;
        IWork<TInput> ITriggeredWork<TInput>.Work => Work;
        public new IActionWork<TInput> Work => work;
        public new IScope<IWorker<TInput>> WorkerScope { get; private set; }

        protected override void CreateWorkerScope()
        {
            WorkerScope = work.Registration.CreateWorkerScope();
        }

        protected override IScope<object> GetWorkerScope()
        {
            return WorkerScope;
        }

        protected override async Task ExecuteWorker()
        {
            await WorkerScope.Worker.Execute(Input);
        }

        protected override void DisposeWorkerScope()
        {
            WorkerScope.Dispose();
        }

        protected override void OnWorkerExecuted()
        {
            work.WorkerExecuted(this);
        }

        protected override void OnWorkerErrored(Exception error)
        {
            work.WorkerErrored(this, error);
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

        IWork ITriggeredWork.Work => Work;
        IWork<TInput> ITriggeredWork<TInput>.Work => Work;
        public new IFuncWork<TInput, TOutput> Work => work;
        public new IScope<IWorker<TInput, TOutput>> WorkerScope { get; private set; }
        public TOutput Output { get; private set; }

        protected override void CreateWorkerScope()
        {
            WorkerScope = work.Registration.CreateWorkerScope();
        }

        protected override IScope<object> GetWorkerScope()
        {
            return WorkerScope;
        }

        protected override async Task ExecuteWorker()
        {
            Output = await WorkerScope.Worker.Execute(Input);
        }

        protected override void DisposeWorkerScope()
        {
            WorkerScope.Dispose();
        }

        protected override void OnWorkerExecuted()
        {
            work.WorkerExecuted(this);
        }

        protected override void OnWorkerErrored(Exception error)
        {
            work.WorkerErrored(this, error);
        }
    }
}
