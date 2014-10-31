using System;
using Flower.Workers;

namespace Flower.Works
{
    internal class WorkRegistration<TInput, TOutput> : WorkRegistrationBase<TInput>, IWorkRegistration<TInput, TOutput>
    {
        public WorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, IWorkerResolver<TInput, TOutput> workerResolver)
            : base(workRegistry, trigger)
        {
            WorkerResolver = workerResolver;
        }

        public IWorkerResolver<TInput, TOutput> WorkerResolver { get; private set; }
    }
}