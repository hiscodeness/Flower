using System;
using System.Reactive.Subjects;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    internal class TestWorkIntToIntSquared : IWork<int, int>, IDisposable
    {
        private readonly TestWorkRegistration registration = new TestWorkRegistration();
        private readonly Subject<int> output = new Subject<int>(); 
        
        internal TestWorkIntToIntSquared()
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
