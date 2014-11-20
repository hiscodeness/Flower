using System;
using System.Reactive.Subjects;
using Flower.Workers;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    internal class TestWorkRegistration : IFuncWorkRegistration<int, int>, IDisposable
    {
        private readonly WorkRegistry workRegistry = new WorkRegistry();
        private readonly Subject<int> trigger = new Subject<int>();

        private readonly Func<IScope<IWorker<int, int>>> createWorkerScope =
            Workers.WorkerScope.Instance(new TestWorkerIntToIntSquared());

        public IWorkRegistry WorkRegistry { get { return workRegistry; } }
        public RegisterOptions Options { get { return workRegistry.DefaultOptions; } }
        public IObservable<int> Trigger { get { return trigger; } }
        public Func<IScope<IWorker<int, int>>> CreateWorkerScope { get { return createWorkerScope; } }
        
        public void Dispose()
        {
            workRegistry.CompleteAll();
            trigger.Dispose();
        }
    }
}