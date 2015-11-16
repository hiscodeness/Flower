using System;
using System.Reactive.Subjects;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    internal class TestWorkRegistration : IFuncWorkRegistration<int, int>, IDisposable
    {
        private readonly WorkRegistry workRegistry = new WorkRegistry();
        private readonly Subject<int> trigger = new Subject<int>();

        private readonly Func<IScope<IWorker<int, int>>> createWorkerScope =
            Workers.WorkerScope.FromInstance(new TestWorkerIntToIntSquared());

        public IWorkRegistry WorkRegistry => workRegistry;
        public WorkOptions Options => workRegistry.Options;
        public IObservable<int> Trigger => trigger;
        public Func<IScope<IWorker<int, int>>> CreateWorkerScope => createWorkerScope;

        public void Dispose()
        {
            workRegistry.CompleteAll();
            trigger.Dispose();
        }
    }
}