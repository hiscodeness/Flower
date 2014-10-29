using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    internal static class TestWorks
    {
        internal static readonly TestWorkIntSquared IntSquaredWork = new TestWorkIntSquared();
    }

    internal class TestWorkInt : IWork<int>, IDisposable
    {
        private readonly TestWorkRegistrationInt registration = new TestWorkRegistrationInt();
        private readonly Subject<int> trigger = new Subject<int>(); 
        private readonly Subject<ITriggeredWork<int>> executed = new Subject<ITriggeredWork<int>>();

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        public void Unregister()
        {
            throw new NotImplementedException();
        }

        public WorkState State { get; private set; }
        public IWorkRegistration<int> Registration { get { return registration; }}
        public Subject<int> Trigger { get { return trigger; } }
        public IObservable<ITriggeredWork<int>> Triggered
        {
            get { return Trigger.Select(input => new TestTriggeredWork<int>(this, input)); }
        }

        public IObservable<ITriggeredWork<int>> Executed
        {
            get { return Trigger.Select(input => new TestTriggeredWork<int>(this, input)); }
        }

        public void Dispose()
        {
            trigger.Dispose();
            executed.Dispose();
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
