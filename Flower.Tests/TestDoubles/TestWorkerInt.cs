﻿namespace Flower.Tests.TestDoubles
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class TestWorkerInt : IWorker<int>
    {
        readonly List<int> inputs = new List<int>(); 

        public void Execute(int input)
        {
            inputs.Add(input);
        }

        public IEnumerable<int> Inputs => inputs;
    }
}
