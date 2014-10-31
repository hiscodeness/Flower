using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class TriggeredWork<TInput, TOutput> : ITriggeredWork<TInput, TOutput>
    {
        private readonly IRegisteredWork<TInput, TOutput> work;

        public TriggeredWork(IWorkRunner workRunner, IRegisteredWork<TInput, TOutput> work, TInput input)
        {
            WorkRunner = workRunner;
            this.work = work;
            Input = input;
            State = TriggeredWorkState.Created;
        }

        public TriggeredWorkState State { get; private set; }
        IWorkBase ITriggeredWorkBase.Work { get { return Work; } }
        IWorkBase<TInput> ITriggeredWorkBase<TInput>.Work { get { return Work; } }
        public IWork<TInput, TOutput> Work { get { return work; } }
        public IWorker<TInput, TOutput> Worker { get; private set; }
        public TInput Input { get; private set; }
        public TOutput Output { get; private set; }
        public IWorkRunner WorkRunner { get; private set; }

        public void Execute()
        {
            try
            {
                State = TriggeredWorkState.Executing;
                Worker = work.Registration.WorkerResolver.Resolve(Input);
                Output = Worker.Execute(Input);
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