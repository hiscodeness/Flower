using System;
using System.Collections.Generic;
using System.Globalization;

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

    internal class TestWorkerInt : IWorker<int>
    {
        readonly List<int> inputs = new List<int>(); 

        public void Execute(int input)
        {
            inputs.Add(input);
        }

        public IEnumerable<int> Inputs { get { return inputs; } } 
    }

    internal class TestWorkerIntToIntSquared : IWorker<int, int>
    {
        public static readonly Func<int, int> WorkerFunc = i => i * i;

        public int Execute(int input)
        {
            return WorkerFunc(input);
        }
    }

    internal class TestWorkerIntToString : IWorker<int, string>
    {
        public string Execute(int input)
        {
            return input.ToString(CultureInfo.InvariantCulture);
        }
    }

    internal class TestWorkerStringToInt : IWorker<string, int>
    {
        public int Execute(string input)
        {
            return int.Parse(input);
        }
    }
}
