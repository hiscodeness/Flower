using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class TriggeredWork : ITriggeredWork
    {
        public TriggeredWork(IWorkRunner workRunner, IWork work, object input)
        {
            WorkRunner = workRunner;
            Work = work;
            Input = input;
            State = TriggeredWorkState.Created;
        }

        public TriggeredWorkState State { get; private set; }
        public IWork Work { get; private set; }
        public IWorker Worker { get; private set; }
        public object Input { get; private set; }

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
                var work = (Work)Work;
                Worker = work.Registration.WorkerResolver.Resolve();
                Worker.Execute();
                work.Registration.WorkerResolver.Release(Worker);
                State = TriggeredWorkState.Success;
            }
            catch (Exception e)
            {
                State = TriggeredWorkState.Failure;
                ((Work)Work).TriggeredWorkErrored(this, e);
                State = TriggeredWorkState.Success;
            }
            finally
            {
                if (State == TriggeredWorkState.Success)
                {
                    ((Work)Work).Observables.TriggeredWorkExecuted(this);
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