using System;
using Flower.Works;

namespace Flower
{
    public static class WorkExtensionMethods
    {
        public static IFuncWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IFuncWork<TPrevInput, TInput> work, Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope, WorkRegistryOptions options = null)
        {
            return work.Registration.WorkRegistry.Register(work.Output, createWorkerScope, options);
        }

        public static IFuncWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IFuncWork<TPrevInput, TInput> work, IWorker<TInput, TOutput> worker, WorkRegistryOptions options = null)
        {
            return work.Registration.WorkRegistry.Register(work.Output, worker, options);
        }
    }
}