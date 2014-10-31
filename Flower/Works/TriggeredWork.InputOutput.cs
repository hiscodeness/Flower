using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class TriggeredWork<TInput, TOutput> : ITriggeredWork<TInput, TOutput>
    {
        public TriggeredWork(IWorkRunner workRunner, IWork<TInput, TOutput> work, TInput input)
        {
            WorkRunner = workRunner;
            Work = work;
            Input = input;
            State = TriggeredWorkState.Created;
        }

        public TriggeredWorkState State { get; private set; }
        public IWork<TInput, TOutput> Work { get; private set; }
        public IWorker<TInput, TOutput> Worker { get; private set; }
        public TInput Input { get; private set; }
        public TOutput Output { get; private set; }

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
                var work = (Work<TInput, TOutput>) Work;
                Worker = work.Registration.WorkerResolver.Resolve(Input);
                Output = Worker.Execute(Input);
                work.Registration.WorkerResolver.Release(Worker);
                State = TriggeredWorkState.Success;
            }
            catch (Exception e)
            {
                State = TriggeredWorkState.Failure;
                ((Work<TInput, TOutput>) Work).TriggeredWorkErrored(this, e);
                State = TriggeredWorkState.Success;
            }
            finally
            {
                if (State == TriggeredWorkState.Success)
                {
                    ((Work<TInput, TOutput>) Work).Observables.TriggeredWorkExecuted(this);
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