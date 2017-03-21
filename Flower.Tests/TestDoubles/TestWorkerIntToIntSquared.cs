namespace Flower.Tests.TestDoubles
{
    using System;
    using System.Threading.Tasks;

    internal class TestWorkerIntToIntSquared : IWorker<int, int>
    {
        public static readonly Func<int, int> WorkerFunc = i => i * i;

        public int Execute(int input)
        {
            return WorkerFunc(input);
        }
    }
}
