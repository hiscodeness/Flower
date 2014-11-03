using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal abstract class TriggeredWork<TInput> : ITriggeredWork<TInput>
    {
        private readonly IRegisteredWork<TInput> work; 
        public TriggeredWorkState State { get; private set; }
        public IWork<TInput> Work { get { return work; } }
        public TInput Input { get; private set; }
        IWork ITriggeredWork.Work { get { return Work; } }
        public IWorkRunner WorkRunner { get; private set; }

        protected TriggeredWork(IWorkRunner workRunner, IRegisteredWork<TInput> work, TInput input)
        {
            WorkRunner = workRunner;
            this.work = work;
            Input = input;
            State = TriggeredWorkState.Pending;
        }

        public void Execute()
        {
            try
            {
                State = TriggeredWorkState.Executing;
                ResolveWorker();
                ExecuteWorker();
                ReleaseWorker();
                State = TriggeredWorkState.Success;
            }
            catch (Exception e)
            {
                State = TriggeredWorkState.Error;
                work.WorkerErrored(e);
                State = TriggeredWorkState.Success;
            }
            finally
            {
                if (State == TriggeredWorkState.Success)
                {
                    WorkerExecuted();
                }
            }
        }

        protected abstract void ResolveWorker();
        protected abstract void ExecuteWorker();
        protected abstract void ReleaseWorker();
        protected abstract void WorkerExecuted();
    }

    internal class TriggeredActionWork : TriggeredWork<object>, ITriggeredActionWork
    {
        private readonly IRegisteredActionWork work;

        public TriggeredActionWork(IWorkRunner workRunner, IRegisteredActionWork work, object input)
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

    internal class TriggeredActionWork<TInput> : TriggeredWork<TInput>, ITriggeredActionWork<TInput>
    {
        private readonly IRegisteredActionWork<TInput> work;

        public TriggeredActionWork(IWorkRunner workRunner, IRegisteredActionWork<TInput> work, TInput input)
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

    internal class TriggeredFuncWork<TInput, TOutput> : TriggeredWork<TInput>, ITriggeredFuncWork<TInput, TOutput>
    {
        private readonly IRegisteredFuncWork<TInput, TOutput> work;

        public TriggeredFuncWork(IWorkRunner workRunner, IRegisteredFuncWork<TInput, TOutput> work, TInput input) : base(workRunner, work, input)
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