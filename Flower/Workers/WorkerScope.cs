using System;

namespace Flower.Workers
{
    public static class WorkerScope
    {
        public static Func<IScope<IWorker>> Instance(IWorker worker)
        {
            return () => new WorkerInstanceScope(worker);
        }

        public static Func<IScope<IWorker<TInput>>> Instance<TInput>(IWorker<TInput> worker)
        {
            return () => new WorkerInstanceScope<TInput>(worker);
        }

        public static Func<IScope<IWorker<TInput, TOutput>>> Instance<TInput, TOutput>(
            IWorker<TInput, TOutput> worker)
        {
            return () => new WorkerInstanceScope<TInput, TOutput>(worker);
        }

        private class WorkerInstanceScope : IScope<IWorker>
        {
            public WorkerInstanceScope(IWorker instance)
            {
                Worker = instance;
            }

            public IWorker Worker { get; private set; }

            public void Dispose()
            {
                // Do nothing, instance is reused
            }
        }

        private class WorkerInstanceScope<TInput> : IScope<IWorker<TInput>>
        {
            public WorkerInstanceScope(IWorker<TInput> instance)
            {
                Worker = instance;
            }

            public IWorker<TInput> Worker { get; private set; }

            public void Dispose()
            {
                // Do nothing, instance is reused
            }
        }

        private class WorkerInstanceScope<TInput, TOutput> : IScope<IWorker<TInput, TOutput>>
        {
            public WorkerInstanceScope(IWorker<TInput, TOutput> instance)
            {
                Worker = instance;
            }

            public IWorker<TInput, TOutput> Worker { get; private set; }

            public void Dispose()
            {
                // Do nothing, instance is reused
            }
        }
    }
}