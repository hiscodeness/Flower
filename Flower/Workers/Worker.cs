using System;

namespace Flower.Workers
{
    using System.Threading.Tasks;

    internal class Worker : IWorker
    {
        private readonly Func<Task> worker;

        public Worker(Func<Task> worker)
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
        private readonly Func<TInput, Task> worker;

        public Worker(Func<TInput, Task> worker)
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
