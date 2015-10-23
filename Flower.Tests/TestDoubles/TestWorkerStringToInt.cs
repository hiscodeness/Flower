namespace Flower.Tests.TestDoubles
{
    internal class TestWorkerStringToInt : IWorker<string, int>
    {
        public int Execute(string input)
        {
            return int.Parse(input);
        }
    }
}
