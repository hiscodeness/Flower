using System;

namespace Flower.Workers
{
    internal class Worker : IWorker
    {
        private readonly Action worker;

        public Worker(Action worker)
        {
            this.worker = worker;
        }

        public void Execute()
        {
            worker();
        }
    }

    internal class Worker<TInput> : IWorker<TInput>
    {
        private readonly Action<TInput> worker;

        public Worker(Action<TInput> worker)
        {
            this.worker = worker;
        }

        public void Execute(TInput input)
        {
            worker(input);
        }
    }

    internal class Worker<TInput, TOutput> : IWorker<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> worker;

        public Worker(Func<TInput, TOutput> worker)
        {
            this.worker = worker;
        }

        public TOutput Execute(TInput input)
        {
            return worker(input);
        }
    }
}
