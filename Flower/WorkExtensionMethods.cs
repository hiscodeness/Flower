using System;
using Flower.Works;

namespace Flower
{
    public static class WorkExtensionMethods
    {
        public static IFuncWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IFuncWork<TPrevInput, TInput> work, Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope, WorkOptions options = null)
        {
            return work.Registration.WorkRegistry.RegisterFactory(work.Output, createWorkerScope, options);
        }

        public static IFuncWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IFuncWork<TPrevInput, TInput> work, IWorker<TInput, TOutput> worker, WorkOptions options = null)
        {
            return work.Registration.WorkRegistry.RegisterWorker(work.Output, worker, options);
        }

        public static IActionWork<TInput> Pipe<TPrevInput, TInput>(
            this IFuncWork<TPrevInput, TInput> work, Func<IScope<IWorker<TInput>>> createWorkerScope, WorkOptions options = null)
        {
            return work.Registration.WorkRegistry.RegisterFactory(work.Output, createWorkerScope, options);
        }

        public static IActionWork<TInput> Pipe<TPrevInput, TInput>(
            this IFuncWork<TPrevInput, TInput> work, IWorker<TInput> worker, WorkOptions options = null)
        {
            return work.Registration.WorkRegistry.RegisterWorker(work.Output, worker, options);
        }

        public static IActionWork Pipe<TPrevInput, TInput>(
            this IFuncWork<TPrevInput, TInput> work, Func<IScope<IWorker>> createWorkerScope, WorkOptions options = null)
        {
            return work.Registration.WorkRegistry.RegisterFactory(work.Output, createWorkerScope, options);
        }

        public static IActionWork Pipe<TPrevInput, TInput>(
            this IFuncWork<TPrevInput, TInput> work, IWorker worker, WorkOptions options = null)
        {
            return work.Registration.WorkRegistry.RegisterWorker(work.Output, worker, options);
        }
    }
}
