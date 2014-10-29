using System;
using System.Collections.Generic;
using System.Globalization;
using Flower.Workers;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    internal static class TestWorkers
    {
        internal static readonly TestWorkerIntSquared IntSquaredWorker = new TestWorkerIntSquared();
        internal static readonly TestWorkerInt2String Int2StringWorker = new TestWorkerInt2String();
        internal static readonly TestWorkerString2Int String2IntWorker = new TestWorkerString2Int();
    }

    internal class TestWorkerTriggeredWork : IWorker<ITriggeredWork<int>>
    {
        readonly List<ITriggeredWork<int>> inputs = new List<ITriggeredWork<int>>();

        public void Execute(ITriggeredWork<int> input)
        {
            inputs.Add(input);
        }

        public IEnumerable<ITriggeredWork<int>> Inputs { get { return inputs; } }
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

    internal class TestWorkerIntSquared : IWorker<int, int>
    {
        public static readonly Func<int, int> WorkerFunc = i => i * i;

        public int Execute(int input)
        {
            return WorkerFunc(input);
        }
    }

    internal class TestWorkerInt2String : IWorker<int, string>
    {
        public string Execute(int input)
        {
            return input.ToString(CultureInfo.InvariantCulture);
        }
    }

    internal class TestWorkerString2Int : IWorker<string, int>
    {
        public int Execute(string input)
        {
            return int.Parse(input);
        }
    }
}
