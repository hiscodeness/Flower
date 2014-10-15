using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Flower.Workers;
using Flower.Works;

namespace Flower.Tests
{
    internal static class TestWorks
    {
        internal static readonly TestWorkIntSquared IntSquaredWork = new TestWorkIntSquared();
    }

    internal class TestWorkIntSquared : IWork<int, int>
    {        
        private readonly WorkRegistry _workRegistry = new WorkRegistry(false);
        private readonly Subject<int> _trigger = new Subject<int>();
        private readonly Subject<int> _output =new Subject<int>(); 
        private readonly IWorkerResolver<int, int> _workerResolver =
            WorkerResolver.CreateFromInstance(TestWorkers.IntSquaredWorker);

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        public IWorkRegistry WorkRegistry { get { return _workRegistry; } }
        public WorkState State { get; private set; }
        IWorkerResolver<int, int> IWork<int, int>.WorkerResolver { get { return _workerResolver; } }
        IObservable<ITriggeredWork<int, int>> IWork<int, int>.Executed { get { return null; } }
        public IObservable<int> Output { get { return _output; } }
        IWorkerResolver<int> IWork<int>.WorkerResolver { get { return null; } }
        IObservable<ITriggeredWork<int>> IWork<int>.Executed { get { return null; } }
        IObservable<int> IWork<int>.Trigger { get { return _trigger; } }
        IWorkerResolver IWork.WorkerResolver { get { return null; } }
        IObservable<object> IWork.Trigger { get { return _trigger.Select(input => input as object); } }
        IObservable<ITriggeredWorkBase> IWork.Executed { get { return null; } }
    }
}
