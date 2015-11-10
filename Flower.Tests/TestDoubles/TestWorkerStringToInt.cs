namespace Flower.Tests.TestDoubles
{
    using System.Threading.Tasks;

    internal class TestWorkerStringToInt : IWorker<string, int>
    {
        public async Task<int> Execute(string input)
        {
            return await Task.FromResult(int.Parse(input));
        }
    }
}
