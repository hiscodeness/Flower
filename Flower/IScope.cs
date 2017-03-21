namespace Flower
{
    using System;

    public interface IScope<out TWorker> : IDisposable
    {
        TWorker Worker { get; }
    }
}
