using System.Reactive.Linq;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public static class WorkExtensionMethods
    {
        public static IWork<TInput> Pipe<TInput>(
              this IWork<TInput> work, IWorkerResolver<TInput> workerResolver)
        {
            return work.Registration.WorkRegistry.Register(work.Executed.Select(w => w.Input), workerResolver);
        }

        public static IWork<TInput> Pipe<TInput>(
            this IWork<TInput> work, IWorker<TInput> worker)
        {
            return work.Registration.WorkRegistry.Register(work.Executed.Select(w => w.Input), worker);
        }

        public static IWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IWork<TPrevInput, TInput> work, IWorkerResolver<TInput, TOutput> workerResolver)
        {
            return work.Registration.WorkRegistry.Register(work.Output, workerResolver);
        }

        public static IWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IWork<TPrevInput, TInput> work, IWorker<TInput, TOutput> worker)
        {
            return work.Registration.WorkRegistry.Register(work.Output, worker);
        }
    }
}