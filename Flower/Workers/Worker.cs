namespace Flower.Workers
{
    using System;
    using System.Threading.Tasks;

    internal class Worker : IWorker
    {
        private readonly Func<Task> worker;

        public Worker(Func<Task> worker)
        {
            this.worker = worker;
        }

        public async Task Execute()
        {
            await worker();
        }
    }

    internal class Worker<TInput> : IWorker<TInput>
    {
        private readonly Func<TInput, Task> worker;

        public Worker(Func<TInput, Task> worker)
        {
            this.worker = worker;
        }

        public async Task Execute(TInput input)
        {
            await worker(input);
        }
    }

    internal class Worker<TInput, TOutput> : IWorker<TInput, TOutput>
    {
        private readonly Func<TInput, Task<TOutput>> worker;

        public Worker(Func<TInput, Task<TOutput>> worker)
        {
            this.worker = worker;
        }

        public async Task<TOutput> Execute(TInput input)
        {
            return await worker(input);
        }
    }
}
