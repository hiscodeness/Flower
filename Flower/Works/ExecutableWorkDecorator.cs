namespace Flower.Works
{
    using System;
    using System.Threading.Tasks;
    using Flower.WorkRunners;

    public abstract class ExecutableWorkDecorator : IExecutableActionWork
    {
        private readonly IExecutableActionWork next;

        protected ExecutableWorkDecorator(IExecutableActionWork next)
        {
            this.next = next;
        }

        public object Input => next.Input;
        public IWork<object> Work => next.Work;
        IWork ITriggeredWork.Work => next.Work;
        public IWorkRunner WorkRunner => next.WorkRunner;
        public ExecutableWorkState State => next.State;

        public virtual async Task Execute()
        {
            await next.Execute();
        }

        public Exception Error => next.Error;
        public IScope<IWorker> WorkerScope => next.WorkerScope;
    }

    public abstract class ExecutableWorkDecorator<TInput> : IExecutableActionWork<TInput>
    {
        private readonly IExecutableActionWork<TInput> next;

        protected ExecutableWorkDecorator(IExecutableActionWork<TInput> next)
        {
            this.next = next;
        }

        public TInput Input => next.Input;
        IActionWork<TInput> ITriggeredActionWork<TInput>.Work => next.Work;
        IWork<TInput> ITriggeredWork<TInput>.Work => next.Work;
        IWork ITriggeredWork.Work => next.Work;
        public IWorkRunner WorkRunner => next.WorkRunner;
        public ExecutableWorkState State => next.State;

        public virtual async Task Execute()
        {
            await next.Execute();
        }

        public Exception Error => next.Error;
        public IScope<IWorker<TInput>> WorkerScope => next.WorkerScope;
    }

    public abstract class ExecutableWorkDecorator<TInput, TOutput> : IExecutableFuncWork<TInput, TOutput>
    {
        private readonly IExecutableFuncWork<TInput, TOutput> next;

        protected ExecutableWorkDecorator(IExecutableFuncWork<TInput, TOutput> next)
        {
            this.next = next;
        }

        public TInput Input => next.Input;
        IFuncWork<TInput, TOutput> ITriggeredFuncWork<TInput, TOutput>.Work => next.Work;
        IWork<TInput> ITriggeredWork<TInput>.Work => next.Work;
        IWork ITriggeredWork.Work => next.Work;
        public IWorkRunner WorkRunner => next.WorkRunner;
        public ExecutableWorkState State => next.State;

        public virtual async Task Execute()
        {
            await next.Execute();
        }

        public Exception Error => next.Error;
        public IScope<IWorker<TInput, TOutput>> WorkerScope => next.WorkerScope;
        public TOutput Output => next.Output;
    }
}
