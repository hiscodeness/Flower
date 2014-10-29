using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class TriggeredWork<TInput> : ITriggeredWork<TInput>
    {
        public TriggeredWork(IWorkRunner workRunner, IWork<TInput> work, TInput input)
        {
            WorkRunner = workRunner;
            Work = work;
            Input = input;
            State = TriggeredWorkState.Created;
        }

        public TriggeredWorkState State { get; private set; }
        public IWork<TInput> Work { get; private set; }
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
                var work = (Work<TInput>)Work;
                Worker = work.Registration.WorkerResolver.Resolve(Input);
                Worker.Execute(Input);
                work.Registration.WorkerResolver.Release(Worker);
                State = TriggeredWorkState.Success;
            }
            catch (Exception e)
            {
                State = TriggeredWorkState.Failure;
                ((Work<TInput>)Work).TriggeredWorkErrored(this, e);
                State = TriggeredWorkState.Success;
            }
            finally
            {
                if (State == TriggeredWorkState.Success)
                {
                    ((Work<TInput>)Work).TriggeredWorkExecuted(this);
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