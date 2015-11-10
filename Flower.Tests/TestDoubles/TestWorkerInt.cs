namespace Flower.Tests.TestDoubles
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class TestWorkerInt : IWorker<int>
    {
        readonly List<int> inputs = new List<int>(); 

        public async Task Execute(int input)
        {
            inputs.Add(input);
            await Task.CompletedTask;
        }

        public IEnumerable<int> Inputs => inputs;
    }
}
