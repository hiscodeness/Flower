namespace Flower.Autofac.Tests
{
    using System;

    public sealed class TestWorkerDisposable : IWorker, IDisposable
    {
        public bool IsExecuted { get; private set; }
        public bool IsDisposed { get; private set; }

        public void Execute()
        {
            IsExecuted = true;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
