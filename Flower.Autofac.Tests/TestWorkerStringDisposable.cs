namespace Flower.Autofac.Tests
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    public sealed class TestWorkerStringDisposable : IWorker<string>, IDisposable
    {
        public int Result { get; private set; }
        public bool IsDisposed { get; private set; }

        public void Execute(string input)
        {
            Result = int.Parse(input, CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
