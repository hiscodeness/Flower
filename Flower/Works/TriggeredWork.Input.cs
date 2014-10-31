using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class TriggeredWork<TInput> : ITriggeredWork<TInput>
    {
        private readonly IRegisteredWork<TInput> work;

        public TriggeredWork(IWorkRunner workRunner, IRegisteredWork<TInput> work, TInput input)
        {
            WorkRunner = workRunner;
            this.work = work;
            Input = input;
            State = TriggeredWorkState.Created;
        }

        public TriggeredWorkState State { get; private set; }
        public IWork<TInput> Work { get { return work; } }
        public IWorker<TInput> Worker { get; private set; }
        public TInput Input { get; private set; }

        IWorkBase ITriggeredWorkBase.Work
        {
            get { return Work; }
        }

        public IWorkRunner WorkRunner { get; private set; }

        public void Execute()
        {
            try
            {
                State = TriggeredWorkState.Executing;
                Worker = work.Registration.WorkerResolver.Resolve(Input);
                Worker.Execute(Input);
                work.Registration.WorkerResolver.Release(Worker);
                State = TriggeredWorkState.Success;
            }
            catch (Exception e)
            {
                State = TriggeredWorkState.Failure;
                work.WorkerErrored(this, e);
                State = TriggeredWorkState.Success;
            }
            finally
            {
                if (State == TriggeredWorkState.Success)
                {
                    work.WorkerExecuted(this);
                }
            }
        }

        public void Submit()
        {
            State = TriggeredWorkState.Submitted;
            WorkRunner.Submit(this);
        }
    }
}