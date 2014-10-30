using System;
using Flower.Workers;

namespace Flower.Works
{
    internal class WorkRegistration<TInput, TOutput> : IWorkRegistration<TInput, TOutput>
    {
        public WorkRegistration(
            IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> workerResolver)
        {
            WorkRegistry = workRegistry;
            Trigger = trigger;
            WorkerResolver = workerResolver;
        }

        public IWorkRegistry WorkRegistry { get; private set; }
        public IObservable<TInput> Trigger { get; private set; }
        public IWorkerResolver<TInput, TOutput> WorkerResolver { get; private set; }
    }
}