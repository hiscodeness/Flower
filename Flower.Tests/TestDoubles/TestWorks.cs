using System;
using System.Reactive.Subjects;
using Flower.Workers;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    internal static class TestWorks
    {
        internal static readonly TestWorkIntSquared IntSquaredWork = new TestWorkIntSquared();
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

    internal class TestWorkIntSquared : IWork<int, int>, IDisposable
    {
        private readonly TestWorkRegistration registration = new TestWorkRegistration();
        private readonly Subject<int> output = new Subject<int>(); 
        
        internal TestWorkIntSquared()
        {
            State = WorkState.Suspended;
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            registration.Dispose();
            output.Dispose();
        }

        public IWorkRegistration<int, int> Registration { get { return registration; } }
        public WorkState State { get; private set; }
        IObservable<ITriggeredWork<int, int>> IWork<int, int>.Executed { get { return null; } }
        public IObservable<int> Output { get { return output; } }
        IObservable<ITriggeredWork<int, int>> IWork<int, int>.Triggered { get { return null; } }
        public void Unregister()
        {
            throw new NotImplementedException();
        }
    }
}
