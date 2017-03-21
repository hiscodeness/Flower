namespace Flower.Tests.TestDoubles
{
    using System.Threading.Tasks;

    internal class TestWorkerStringToInt : IWorker<string, int>
    {
        public int Execute(string input)
        {
            return int.Parse(input);
        }
    }
}
