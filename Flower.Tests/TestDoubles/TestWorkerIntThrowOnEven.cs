using System;

namespace Flower.Tests.TestDoubles
{
    using System.Threading.Tasks;

    internal class TestWorkerIntThrowOnEven : IWorker<int>
    {
        public const string ErrorMessage = "Even numbers throw exception.";

        public void Execute(int input)
        {
            if (input % 2 == 0)
            {
                throw new ArgumentException(ErrorMessage);
            }
        }
    }
}
