﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace Flower.Tests.TestDoubles
{
    internal class TestWorkerThrowsException : IWorker
    {
        public void Execute()
        {
            throw new Exception(ErrorMessage);
        }

        public static string ErrorMessage
        {
            get { return "Test worker exception."; }
        }
    }

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

    internal class TestWorkerIntToIntThrowOnEven : IWorker<int, int>
    {
        public const string ErrorMessage = "Even numbers throw exception.";
        public static readonly Func<int, int> WorkerFunc = i => i;

        public int Execute(int input)
        {
            if (input%2 == 0)
            {
                throw new InvalidOperationException(ErrorMessage);
            }

            return WorkerFunc(input);
        }
    }
}
