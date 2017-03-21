namespace Flower.Autofac.Tests
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    public sealed class TestWorkerStringDisposable : IWorker<string>, IDisposable
    {
        public int Result { get; private set; }
        public bool IsDisposed { get; private set; }

        public async Task Execute(string input)
        {
            Result = int.Parse(input, CultureInfo.InvariantCulture);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
