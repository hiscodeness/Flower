using System;
using Flower.WorkRunners;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    internal class TestTriggeredWork<TInput> : ITriggeredWork<TInput>
    {      
        public TriggeredWorkState State { get; private set; }
        public IWork<TInput> Work { get; private set; }
        public IWorker<TInput> Worker { get; private set; }
        public TInput Input { get; private set; }

        public TestTriggeredWork(IWork<TInput> work, TInput input)
        {
            Work = work;
            Input = input;
        }

        IWorkBase ITriggeredWorkBase.Work
        {
            get { return Work; }
        }

        public IWorkRunner WorkRunner { get; private set; }
        public void Submit()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}