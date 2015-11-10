namespace Flower.Autofac.Tests
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    public sealed class TestWorkerStringToIntDisposable : IWorker<string, int>, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public async Task<int> Execute(string input)
        {
            return await Task.FromResult(int.Parse(input, CultureInfo.InvariantCulture));
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
