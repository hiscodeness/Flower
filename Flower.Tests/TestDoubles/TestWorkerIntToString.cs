namespace Flower.Tests.TestDoubles
{
    using System.Globalization;

    internal class TestWorkerIntToString : IWorker<int, string>
    {
        public string Execute(int input)
        {
            return input.ToString(CultureInfo.InvariantCulture);
        }
    }
}
