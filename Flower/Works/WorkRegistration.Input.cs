using System;
using Flower.Workers;

namespace Flower.Works
{
    internal class WorkRegistration<TInput> : WorkRegistrationBase<TInput>, IWorkRegistration<TInput>
    {
        public WorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, IWorkerResolver<TInput> workerResolver)
            : base(workRegistry, trigger)
        {
            WorkerResolver = workerResolver;
        }

        public IWorkerResolver<TInput> WorkerResolver { get; private set; }
    }
}