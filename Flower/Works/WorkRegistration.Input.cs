using System;
using Flower.Workers;

namespace Flower.Works
{
    internal class WorkRegistration<TInput> : IWorkRegistration<TInput>
    {
        public WorkRegistration(
            IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            IWorkerResolver<TInput> workerResolver)
        {
            WorkRegistry = workRegistry;
            Trigger = trigger;
            WorkerResolver = workerResolver;
        }

        public IWorkRegistry WorkRegistry { get; private set; }
        public IObservable<TInput> Trigger { get; private set; }
        public IWorkerResolver<TInput> WorkerResolver { get; private set; }

        public void Dispose()
        {
            WorkRegistry.Dispose();
        }
    }
}