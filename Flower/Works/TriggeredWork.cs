using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class TriggeredWork : ITriggeredWork
    {
        private readonly IRegisteredWork work;

        public TriggeredWork(IWorkRunner workRunner, IRegisteredWork work, object input)
        {
            WorkRunner = workRunner;
            this.work = work;
            Input = input;
            State = TriggeredWorkState.Created;
        }

        public TriggeredWorkState State { get; private set; }
        public IWorkBase<object> Work { get { return work; } }
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
                Worker = work.Registration.WorkerResolver.Resolve();
                Worker.Execute();
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