using System;

namespace Flower
{
    public interface IScope<out TWorker> : IDisposable
    {
        TWorker Worker { get; } 
    }
}