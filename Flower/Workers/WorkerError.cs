namespace Flower.Workers
{
    using System;

    public class WorkerError
    {
        public WorkerError(Exception error, object worker)
        {
            Worker = worker;
            Error = error;
        }

        public Exception Error { get; }

        public object Worker { get; }
    }
}