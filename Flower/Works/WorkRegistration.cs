using System;
using Flower.Workers;

namespace Flower.Works
{
    internal class WorkRegistration : WorkRegistrationBase<object>, IWorkRegistration
    {
        public WorkRegistration(IWorkRegistry workRegistry, IObservable<object> trigger, IWorkerResolver workerResolver)
            : base(workRegistry, trigger)
        {
            WorkerResolver = workerResolver;
        }

        public IWorkerResolver WorkerResolver { get; private set; }
    }
}