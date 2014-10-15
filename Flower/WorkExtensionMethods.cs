using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public static class WorkExtensionMethods
    {
        public static IWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IWork<TPrevInput, TInput> work, IWorkerResolver<TInput, TOutput> workerResolver)
        {
            return work.WorkRegistry.Register(work.Output, workerResolver);
        }

        public static IWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IWork<TPrevInput, TInput> work, IWorker<TInput, TOutput> worker)
        {
            return work.WorkRegistry.Register(work.Output, worker);
        }
    }
}