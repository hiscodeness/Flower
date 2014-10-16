namespace Flower.Workers
{
    public static class WorkerResolver
    {
        public static IWorkerResolver<TInput, TOutput> CreateFromInstance<TInput, TOutput>(
            IWorker<TInput, TOutput> worker)
        {
            return new InstanceWorkerResolver<TInput, TOutput>(worker);
        }

        private class InstanceWorkerResolver<TInput, TOutput> : IWorkerResolver<TInput, TOutput>
        {
            private readonly IWorker<TInput, TOutput> _instance;

            public InstanceWorkerResolver(IWorker<TInput, TOutput> instance)
            {
                _instance = instance;
            }

            public void Release(IWorker<TInput, TOutput> worker)
            {
                // Do nothing, instance is reused every time
            }

            public IWorker<TInput, TOutput> Resolve(TInput input)
            {
                return _instance;
            }
        }
    }
}