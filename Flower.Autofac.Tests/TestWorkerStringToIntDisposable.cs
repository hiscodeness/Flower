namespace Flower.Autofac.Tests
{
    using System;
    using System.Globalization;

    public sealed class TestWorkerStringToIntDisposable : IWorker<string, int>, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public int Execute(string input)
        {
            return int.Parse(input, CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
