namespace Flower.Workers
{
    using System;

    public interface IWorkerError
    {
        Exception Error { get; }

        object Worker { get; }
    }
}
