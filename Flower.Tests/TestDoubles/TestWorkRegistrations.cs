using System;
using System.Reactive.Subjects;
using Flower.Workers;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    internal class TestWorkRegistration : IWorkRegistration<int, int>, IDisposable
    {
        private readonly WorkRegistry workRegistry = new WorkRegistry();
        private readonly Subject<int> trigger = new Subject<int>();

        private readonly IWorkerResolver<int, int> workerResolver =
            Workers.WorkerResolver.CreateFromInstance(new TestWorkerIntToIntSquared());

        public IWorkRegistry WorkRegistry { get { return workRegistry; } }
        public WorkRegistryOptions Options { get { return workRegistry.Options; } }
        public IObservable<int> Trigger { get { return trigger; } }
        public IWorkerResolver<int, int> WorkerResolver { get { return workerResolver; } }
        
        public void Dispose()
        {
            workRegistry.Dispose();
            trigger.Dispose();
        }
    }
}