namespace Flower.Tests.TestDoubles
{
    using System.Globalization;
    using System.Threading.Tasks;

    internal class TestWorkerIntToString : IWorker<int, string>
    {
        public async Task<string> Execute(int input)
        {
            return await Task.FromResult(input.ToString(CultureInfo.InvariantCulture));
        }
    }
}
