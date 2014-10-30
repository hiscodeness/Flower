using System;
using Flower.Workers;

namespace Flower.Works
{
    internal class WorkRegistration : IWorkRegistration
    {
        public WorkRegistration(
            IWorkRegistry workRegistry,
            IObservable<object> trigger,
            IWorkerResolver workerResolver)
        {
            WorkRegistry = workRegistry;
            Trigger = trigger;
            WorkerResolver = workerResolver;
        }

        public IWorkRegistry WorkRegistry { get; private set; }
        public IObservable<object> Trigger { get; private set; }
        public IWorkerResolver WorkerResolver { get; private set; }
    }
}