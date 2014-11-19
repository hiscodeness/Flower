using System;
using System.Reactive.Linq;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public static class WorkExtensionMethods
    {
        public static IFuncWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IFuncWork<TPrevInput, TInput> work, Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope)
        {
            return work.Registration.WorkRegistry.Register(work.Output, createWorkerScope);
        }

        public static IFuncWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IFuncWork<TPrevInput, TInput> work, IWorker<TInput, TOutput> worker)
        {
            return work.Registration.WorkRegistry.Register(work.Output, worker);
        }
    }
}