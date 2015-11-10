namespace Flower.Autofac.Tests
{
    using System;
    using System.Threading.Tasks;

    public sealed class TestWorkerDisposable : IWorker, IDisposable
    {
        public bool IsExecuted { get; private set; }
        public bool IsDisposed { get; private set; }

        public async Task Execute()
        {
            IsExecuted = true;
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
