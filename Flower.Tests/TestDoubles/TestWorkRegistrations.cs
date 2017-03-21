namespace Flower.Tests.TestDoubles
{
    using System;
    using System.Reactive.Subjects;
    using Flower.Workers;
    using Flower.Works;

    internal class TestWorkRegistration : IFuncWorkRegistration<int, int>, IDisposable
    {
        private readonly WorkRegistry workRegistry = new WorkRegistry();
        private readonly Subject<int> trigger = new Subject<int>();

        public IWorkRegistry WorkRegistry => workRegistry;
        public WorkOptions Options => workRegistry.Options;
        public IObservable<int> Trigger => trigger;

        public Func<IScope<IWorker<int, int>>> CreateWorkerScope { get; } =
            WorkerScope.FromInstance(new TestWorkerIntToIntSquared());

        public void Dispose()
        {
            workRegistry.CompleteAll();
            trigger.Dispose();
        }
    }
}
