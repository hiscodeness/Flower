namespace Flower.Workers
{
    using System;

    public abstract class WorkerErrorBase : IWorkerError
    {
        protected WorkerErrorBase(Exception error, object worker)
        {
            Worker = worker;
            Error = error;
        }

        public Exception Error { get; }

        public object Worker { get; }
    }

    public class WorkerError : WorkerErrorBase
    {
        public WorkerError(Exception error, IWorker worker)
            : base(error, worker)
        {
            Worker = worker;
        }

        public new IWorker Worker { get; }
    }


    public class WorkerError<TInput> : WorkerErrorBase
    {
        public WorkerError(Exception error, IWorker<TInput> worker)
            : base(error, worker)
        {
            Worker = worker;
        }

        public new IWorker<TInput> Worker { get; }
    }

    public class WorkerError<TInput, TOutput> : WorkerErrorBase
    {
        public WorkerError(Exception error, IWorker<TInput, TOutput> worker)
            : base(error, worker)
        {
            Worker = worker;
        }

        public new IWorker<TInput, TOutput> Worker { get; }
    }
}
