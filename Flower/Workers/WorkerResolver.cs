namespace Flower.Workers
{
    public static class WorkerResolver
    {
        public static IWorkerResolver<TInput> CreateFromInstance<TInput>(
            IWorker<TInput> worker)
        {
            return new InstanceWorkerResolver<TInput>(worker);
        }

        public static IWorkerResolver<TInput, TOutput> CreateFromInstance<TInput, TOutput>(
            IWorker<TInput, TOutput> worker)
        {
            return new InstanceWorkerResolver<TInput, TOutput>(worker);
        }

        private class InstanceWorkerResolver<TInput> : IWorkerResolver<TInput>
        {
            private readonly IWorker<TInput> instance;

            public InstanceWorkerResolver(IWorker<TInput> instance)
            {
                this.instance = instance;
            }

            public void Release(IWorker<TInput> worker)
            {
                // Do nothing, instance is reused every time
            }

            public IWorker<TInput> Resolve(TInput input)
            {
                return instance;
            }
        }

        private class InstanceWorkerResolver<TInput, TOutput> : IWorkerResolver<TInput, TOutput>
        {
            private readonly IWorker<TInput, TOutput> instance;

            public InstanceWorkerResolver(IWorker<TInput, TOutput> instance)
            {
                this.instance = instance;
            }

            public void Release(IWorker<TInput, TOutput> worker)
            {
                // Do nothing, instance is reused every time
            }

            public IWorker<TInput, TOutput> Resolve(TInput input)
            {
                return instance;
            }
        }
    }
}