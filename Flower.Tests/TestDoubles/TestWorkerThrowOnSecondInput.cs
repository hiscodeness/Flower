namespace Flower.Tests.TestDoubles
{
    using System;
    using System.Threading.Tasks;

    internal class TestWorkerThrowOnSecondInput : IWorker
    {
        public const string ErrorMessage = "Second input throw exception.";
        private int inputCount;


        public async Task Execute()
        {
            inputCount++;
            if (inputCount == 2)
            {
                throw new ArgumentException(ErrorMessage);
            }

            await Task.CompletedTask;
        }
    }
}
