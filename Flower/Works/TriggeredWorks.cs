using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class TriggeredActionWork : ITriggeredActionWork
    {
        private readonly IRegisteredActionWork work;

        public TriggeredActionWork(IWorkRunner workRunner, IRegisteredActionWork work, object input)
        {
            WorkRunner = workRunner;
            this.work = work;
            Input = input;
            State = TriggeredWorkState.Created;
        }

        public TriggeredWorkState State { get; private set; }
        public IWork<object> Work { get { return work; } }
        public IWorker Worker { get; private set; }
        public object Input { get; private set; }

        IWork ITriggeredWork.Work
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
                work.WorkerErrored(e);
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
    }

    internal class TriggeredActionWork<TInput> : ITriggeredActionWork<TInput>
    {
        private readonly IRegisteredActionWork<TInput> work;

        public TriggeredActionWork(IWorkRunner workRunner, IRegisteredActionWork<TInput> work, TInput input)
        {
            WorkRunner = workRunner;
            this.work = work;
            Input = input;
            State = TriggeredWorkState.Created;
        }

        public TriggeredWorkState State { get; private set; }
        IWork ITriggeredWork.Work { get { return Work; } }
        IWork<TInput> ITriggeredWork<TInput>.Work { get { return Work; } }
        public IActionWork<TInput> Work { get { return work; } }
        public IWorker<TInput> Worker { get; private set; }
        public TInput Input { get; private set; }
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
                work.WorkerErrored(e);
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
    }

    internal class TriggeredFuncWork<TInput, TOutput> : ITriggeredFuncWork<TInput, TOutput>
    {
        private readonly IRegisteredFuncWork<TInput, TOutput> work;

        public TriggeredFuncWork(IWorkRunner workRunner, IRegisteredFuncWork<TInput, TOutput> work, TInput input)
        {
            WorkRunner = workRunner;
            this.work = work;
            Input = input;
            State = TriggeredWorkState.Created;
        }

        public TriggeredWorkState State { get; private set; }
        IWork ITriggeredWork.Work { get { return Work; } }
        IWork<TInput> ITriggeredWork<TInput>.Work { get { return Work; } }
        public IFuncWork<TInput, TOutput> Work { get { return work; } }
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
                work.WorkerErrored(e);
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
    }
}