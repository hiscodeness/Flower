namespace Flower.Workers
{
    internal class WorkerResolverScope : IScope<IWorker>
    {
        private readonly IWorkerResolver resolver;

        public WorkerResolverScope(IWorkerResolver resolver)
        {
            this.resolver = resolver;
            Worker = resolver.Resolve();
        }

        public IWorker Worker { get; }

        public void Dispose()
        {
            resolver.Release(Worker);
        }
    }

    internal class WorkerResolverScope<TInput> : IScope<IWorker<TInput>>
    {
        private readonly IWorkerResolver<TInput> resolver;

        public WorkerResolverScope(IWorkerResolver<TInput> resolver)
        {
            this.resolver = resolver;
            Worker = resolver.Resolve();
        }

        public IWorker<TInput> Worker { get; }

        public void Dispose()
        {
            resolver.Release(Worker);
        }
    }

    internal class WorkerResolverScope<TInput, TOutput> : IScope<IWorker<TInput, TOutput>>
    {
        private readonly IWorkerResolver<TInput, TOutput> resolver;

        public WorkerResolverScope(IWorkerResolver<TInput, TOutput> resolver)
        {
            this.resolver = resolver;
            Worker = resolver.Resolve();
        }

        public IWorker<TInput, TOutput> Worker { get; }

        public void Dispose()
        {
            resolver.Release(Worker);
        }
    }
}
