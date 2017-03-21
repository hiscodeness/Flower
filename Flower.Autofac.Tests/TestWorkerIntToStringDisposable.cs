namespace Flower.Autofac.Tests
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    public sealed class TestWorkerIntToStringDisposable : IWorker<int, string>, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public Task<string> Execute(int input)
        {
            return Task.FromResult(input.ToString(CultureInfo.InvariantCulture));
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
