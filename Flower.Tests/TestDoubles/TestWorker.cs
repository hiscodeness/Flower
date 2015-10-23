namespace Flower.Tests.TestDoubles
{
    internal class TestWorker : IWorker
    {
        public void Execute()
        {
            ExecuteCount++;
        }

        public int ExecuteCount { get; private set; }
    }
}
