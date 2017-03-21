namespace Flower.Tests.TestDoubles
{
    using System;
    using System.Threading.Tasks;

    internal class TestWorkerIntThrowOnEven : IWorker<int>
    {
        public const string ErrorMessage = "Even numbers throw exception.";

        public async Task Execute(int input)
        {
            if (input % 2 == 0)
            {
                throw new ArgumentException(ErrorMessage);
            }

            await Task.CompletedTask;
        }
    }
}
