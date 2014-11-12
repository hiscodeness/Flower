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
        public Exception Exception { get; private set; }

        protected ExecutableWork(IWorkRunner workRunner, IRegisteredWork<TInput> work, TInput input)
        {
            WorkRunner = workRunner;
            this.work = work;
            Input = input;
            State = ExecutableWorkState.Pending;
        }

        public void Execute()
        {
            try
            {
                State = ExecutableWorkState.Executing;
                ResolveWorker();
                ExecuteWorker();
                ReleaseWorker();
                State = ExecutableWorkState.Success;
            }
            catch (Exception e)
            {
                State = ExecutableWorkState.Error;
                Exception = e;
                work.WorkerErrored(e);
            }
            finally
            {
                if (ShouldNotifyWorkerExecuted())
                {
                    WorkerExecuted();
                }
            }
        }

        private bool ShouldNotifyWorkerExecuted()
        {
            switch (State)
            {
                case ExecutableWorkState.Success:
                    return true;
                case ExecutableWorkState.Error:
                    return work.Registration.Options.WorkerErrorBehavior == WorkerErrorBehavior.NotifyExecuted;
                default:
                    return false;
            }
        }

        protected abstract void ResolveWorker();
        protected abstract void ExecuteWorker();
        protected abstract void ReleaseWorker();
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

        public IWorker Worker { get; private set; }

        protected override void ResolveWorker()
        {
            Worker = work.Registration.WorkerResolver.Resolve();
        }

        protected override void ExecuteWorker()
        {
            Worker.Execute();
        }

        protected override void ReleaseWorker()
        {
            work.Registration.WorkerResolver.Release(Worker);
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
        public IWorker<TInput> Worker { get; private set; }

        protected override void ResolveWorker()
        {
            Worker = work.Registration.WorkerResolver.Resolve(Input);
        }

        protected override void ExecuteWorker()
        {
            Worker.Execute(Input);
        }

        protected override void ReleaseWorker()
        {
            work.Registration.WorkerResolver.Release(Worker);
        }

        protected override void WorkerExecuted()
        {
            work.WorkerExecuted(this);
        }
    }

    internal class ExecutableFuncWork<TInput, TOutput> : ExecutableWork<TInput>, IExecutableFuncWork<TInput, TOutput>
    {
        private readonly IRegisteredFuncWork<TInput, TOutput> work;

        public ExecutableFuncWork(IWorkRunner workRunner, IRegisteredFuncWork<TInput, TOutput> work, TInput input) : base(workRunner, work, input)
        {
            this.work = work;
        }

        IWork ITriggeredWork.Work { get { return Work; } }
        IWork<TInput> ITriggeredWork<TInput>.Work { get { return Work; } }
        public new IFuncWork<TInput, TOutput> Work { get { return work; } }
        public IWorker<TInput, TOutput> Worker { get; private set; }
        public TOutput Output { get; private set; }

        protected override void ResolveWorker()
        {
            Worker = work.Registration.WorkerResolver.Resolve(Input);
        }

        protected override void ExecuteWorker()
        {
            Output = Worker.Execute(Input);
        }

        protected override void ReleaseWorker()
        {
            work.Registration.WorkerResolver.Release(Worker);
        }

        protected override void WorkerExecuted()
        {
            work.WorkerExecuted(this);
        }
    }
}