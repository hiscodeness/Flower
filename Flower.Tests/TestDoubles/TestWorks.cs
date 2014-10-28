using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Flower.Workers;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    internal static class TestWorks
    {
        internal static readonly TestWorkIntSquared IntSquaredWork = new TestWorkIntSquared();
    }

    internal class TestWorkIntSquared : IWork<int, int>, IDisposable
    {        
        private readonly WorkRegistry workRegistry = new WorkRegistry();
        private readonly Subject<int> trigger = new Subject<int>();
        private readonly Subject<int> output =new Subject<int>(); 
        private readonly IWorkerResolver<int, int> workerResolver =
            WorkerResolver.CreateFromInstance(TestWorkers.IntSquaredWorker);

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
            workRegistry.Dispose();
            trigger.Dispose();
            output.Dispose();
        }

        public WorkRegistryOptions Options { get { return new WorkRegistryOptions(); } }
        public IWorkRegistry WorkRegistry { get { return workRegistry; } }
        public WorkState State { get; private set; }
        IWorkerResolver<int, int> IWork<int, int>.WorkerResolver { get { return workerResolver; } }
        IObservable<ITriggeredWork<int, int>> IWork<int, int>.Executed { get { return null; } }
        public IObservable<int> Output { get { return output; } }
        IWorkerResolver<int> IWork<int>.WorkerResolver { get { return null; } }
        IObservable<ITriggeredWork<int>> IWork<int>.Executed { get { return null; } }
        IObservable<int> IWork<int>.Trigger { get { return trigger; } }
        IWorkerResolver IWork.WorkerResolver { get { return null; } }
        IObservable<object> IWork.Trigger { get { return trigger.Select(input => input as object); } }
        IObservable<ITriggeredWork<int, int>> IWork<int, int>.Triggered { get { return null; } }
        IObservable<ITriggeredWorkBase> IWork.Executed { get { return null; } }
        public void Unregister()
        {
            throw new NotImplementedException();
        }
    }
}
