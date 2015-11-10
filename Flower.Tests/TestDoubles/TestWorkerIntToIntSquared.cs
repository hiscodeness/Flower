namespace Flower.Tests.TestDoubles
{
    using System;
    using System.Threading.Tasks;

    internal class TestWorkerIntToIntSquared : IWorker<int, int>
    {
        public static readonly Func<int, int> WorkerFunc = i => i * i;

        public async Task<int> Execute(int input)
        {
            return await Task.FromResult(WorkerFunc(input));
        }
    }
}
