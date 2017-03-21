namespace Flower.Tests.TestDoubles
{
    using System;
    using System.Threading.Tasks;

    internal class TestWorkerIntToIntThrowOnEven : IWorker<int, int>
    {
        public const string ErrorMessage = "Even numbers throw exception.";
        public static readonly Func<int, int> WorkerFunc = i => i;

        public async Task<int> Execute(int input)
        {
            if (input % 2 == 0)
            {
                throw new ArgumentException(ErrorMessage);
            }

            return await Task.FromResult(WorkerFunc(input));
        }
    }
}
