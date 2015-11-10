namespace Flower.Tests.TestDoubles
{
    using System.Threading.Tasks;

    internal class TestWorker : IWorker
    {
        public async Task Execute()
        {
            ExecuteCount++;
            await Task.CompletedTask;
        }

        public int ExecuteCount { get; private set; }
    }
}
