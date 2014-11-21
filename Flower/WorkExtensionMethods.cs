using System;
using Flower.Works;

namespace Flower
{
    public static class WorkExtensionMethods
    {
        public static IFuncWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IFuncWork<TPrevInput, TInput> work, Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope, RegisterOptions options = null)
        {
            return work.Registration.WorkRegistry.Register(work.Output, createWorkerScope, options);
        }

        public static IFuncWork<TInput, TOutput> Pipe<TPrevInput, TInput, TOutput>(
            this IFuncWork<TPrevInput, TInput> work, IWorker<TInput, TOutput> worker, RegisterOptions options = null)
        {
            return work.Registration.WorkRegistry.Register(work.Output, worker, options);
        }

        public static IActionWork<TInput> Pipe<TPrevInput, TInput>(
            this IFuncWork<TPrevInput, TInput> work, Func<IScope<IWorker<TInput>>> createWorkerScope, RegisterOptions options = null)
        {
            return work.Registration.WorkRegistry.Register(work.Output, createWorkerScope, options);
        }

        public static IActionWork<TInput> Pipe<TPrevInput, TInput>(
            this IFuncWork<TPrevInput, TInput> work, IWorker<TInput> worker, RegisterOptions options = null)
        {
            return work.Registration.WorkRegistry.Register(work.Output, worker, options);
        }

        public static IActionWork Pipe<TPrevInput, TInput>(
            this IFuncWork<TPrevInput, TInput> work, Func<IScope<IWorker>> createWorkerScope, RegisterOptions options = null)
        {
            return work.Registration.WorkRegistry.Register(work.Output, createWorkerScope, options);
        }

        public static IActionWork Pipe<TPrevInput, TInput>(
            this IFuncWork<TPrevInput, TInput> work, IWorker worker, RegisterOptions options = null)
        {
            return work.Registration.WorkRegistry.Register(work.Output, worker, options);
        }
    }
}