using System;

namespace Flower.Tests.TestDoubles
{
    using System.Threading.Tasks;

    internal class TestWorkerIntToIntThrowOnEven : IWorker<int, int>
    {
        public const string ErrorMessage = "Even numbers throw exception.";
        public static readonly Func<int, int> WorkerFunc = i => i;

        public int Execute(int input)
        {
            if (input%2 == 0)
            {
                throw new ArgumentException(ErrorMessage);
            }

            return WorkerFunc(input);
        }
    }
}
