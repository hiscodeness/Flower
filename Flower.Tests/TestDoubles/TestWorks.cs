using System;
using System.Reactive.Subjects;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    using Flower.Workers;

    internal class TestWorkIntToIntSquared : IFuncWork<int, int>, IDisposable
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

        public IFuncWorkRegistration<int, int> Registration => registration;
        public WorkState State { get; }
        IWorkRegistration<int> IWork<int>.Registration => Registration;
        IWorkRegistration IWork.Registration => Registration;
        IObservable<IExecutableWork> IWork.Executed => null;
        IObservable<IExecutableFuncWork<int, int>> IFuncWork<int, int>.Executed => null;
        IObservable<IWork> IWork.Completed => null;
        public WorkerError LastError => null;
        IObservable<IFuncWork<int, int>> IFuncWork<int, int>.Completed => null;
        public IObservable<int> Output => output;
        IObservable<ITriggeredFuncWork<int, int>> IFuncWork<int, int>.Triggered => null;

        public void Complete()
        {
            throw new NotImplementedException();
        }
    }
}
