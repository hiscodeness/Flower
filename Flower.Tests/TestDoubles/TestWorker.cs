namespace Flower.Tests.TestDoubles
{
    using System.Threading.Tasks;

    internal class TestWorker : IWorker
    {
        public void Execute()
        {
            ExecuteCount++;
        }

        public int ExecuteCount { get; private set; }
    }
}
