using System;

namespace Flower.Workers
{
    public static class WorkerScope
    {
        public static IScope<IWorker> FromResolver(IWorkerResolver resolver)
        {
            return new WorkerResolverScope(resolver);
        }

        public static IScope<IWorker<TInput>> FromResolver<TInput>(IWorkerResolver<TInput> resolver)
        {
            return new WorkerResolverScope<TInput>(resolver);
        }

        public static IScope<IWorker<TInput, TOutput>> FromResolver<TInput, TOutput>(IWorkerResolver<TInput, TOutput> resolver)
        {
            return new WorkerResolverScope<TInput, TOutput>(resolver);
        }

        public static IScope<IWorker> FromInstance(IWorker worker)
        {
            return new WorkerInstanceScope(worker);
        }

        public static Func<IScope<IWorker<TInput>>> FromInstance<TInput>(IWorker<TInput> worker)
        {
            return () => new WorkerInstanceScope<TInput>(worker);
        }

        public static Func<IScope<IWorker<TInput, TOutput>>> FromInstance<TInput, TOutput>(
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

            public IWorker Worker { get; }

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

            public IWorker<TInput> Worker { get; }

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

            public IWorker<TInput, TOutput> Worker { get; }

            public void Dispose()
            {
                // Do nothing, instance is reused
            }
        }
    }
}
