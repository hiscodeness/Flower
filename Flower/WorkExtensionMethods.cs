using System.Reactive.Linq;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public static class WorkExtensionMethods
    {
        public static IActionWork<TInput> Pipe<TInput>(
              this IActionWork<TInput> work, IWorkerResolver<TInput> workerResolver)
        {
            return work.Registration.WorkRegistry.Register(work.Executed.Select(w => w.Input), workerResolver);
        }

        public static IActionWork<TInput> Pipe<TInput>(
            this IActionWork<TInput> work, IWorker<TInput> worker)
        {
            return work.Registration.WorkRegistry.Register(work.Executed.Select(w => w.Input), worker);
        }

        public static IFuncWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IFuncWork<TPrevInput, TInput> work, IWorkerResolver<TInput, TOutput> workerResolver)
        {
            return work.Registration.WorkRegistry.Register(work.Output, workerResolver);
        }

        public static IFuncWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IFuncWork<TPrevInput, TInput> work, IWorker<TInput, TOutput> worker)
        {
            return work.Registration.WorkRegistry.Register(work.Output, worker);
        }
    }
}