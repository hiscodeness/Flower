namespace Flower.Autofac.Tests
{
    using System;
    using System.Globalization;

    public sealed class TestWorkerIntToStringDisposable : IWorker<int, string>, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public string Execute(int input)
        {
            return input.ToString(CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
