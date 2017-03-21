using System;

namespace Flower.Tests.TestDoubles
{
    using System.Threading.Tasks;

    internal class TestWorkerThrowOnSecondInput : IWorker
    {
        public const string ErrorMessage = "Second input throw exception.";
        private int inputCount;


        public void Execute()
        {
            inputCount++;
            if (inputCount == 2)
            {
                throw new ArgumentException(ErrorMessage);
            }
        }
    }
}
