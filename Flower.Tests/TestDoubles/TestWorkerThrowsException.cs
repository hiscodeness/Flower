namespace Flower.Tests.TestDoubles
{
    using System;
    using System.Threading.Tasks;

    internal class TestWorkerThrowsException : IWorker
    {
        public async Task Execute()
        {
            await Task.CompletedTask;
            throw new Exception(ErrorMessage);
        }

        public static string ErrorMessage => "Test worker exception.";
    }
}
