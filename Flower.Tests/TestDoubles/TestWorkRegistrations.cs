using System;
using System.Reactive.Subjects;
using Flower.Workers;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    internal class TestWorkRegistrationInt : IWorkRegistration<int>
    {
        private readonly WorkRegistry workRegistry = new WorkRegistry();
        private readonly Subject<int> trigger = new Subject<int>();

        private readonly IWorkerResolver<int> workerResolver =
            Workers.WorkerResolver.CreateFromInstance(new TestWorkerInt());

        public IWorkRegistry WorkRegistry { get { return workRegistry; } }
        public IObservable<int> Trigger { get { return trigger; } }
        public IWorkerResolver<int> WorkerResolver { get { return workerResolver; } }

        public void Dispose()
        {
            workRegistry.Dispose();
            trigger.Dispose();
        }
    }
    
    internal class TestWorkRegistration : IWorkRegistration<int, int>
    {
        private readonly WorkRegistry workRegistry = new WorkRegistry();
        private readonly Subject<int> trigger = new Subject<int>();

        private readonly IWorkerResolver<int, int> workerResolver =
            Workers.WorkerResolver.CreateFromInstance(TestWorkers.IntSquaredWorker);

        public IWorkRegistry WorkRegistry { get { return workRegistry; } }
        public IObservable<int> Trigger { get { return trigger; } }
        public IWorkerResolver<int, int> WorkerResolver { get { return workerResolver; } }
        
        public void Dispose()
        {
            workRegistry.Dispose();
            trigger.Dispose();
        }
    }
}