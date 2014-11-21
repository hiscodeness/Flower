using System;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public static class WorkRegistryExtensionMethods
    {
        public static IActionWork Register<TInput>(
            this IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            Action worker,
            RegisterOptions options = null)
        {
            return workRegistry.Register(trigger, WorkerScope.Instance(new Worker(worker)), options);
        }

        public static IActionWork<TInput> Register<TInput>(
            this IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            Action<TInput> worker,
            RegisterOptions options = null)
        {
            return workRegistry.Register(trigger, WorkerScope.Instance(new Worker<TInput>(worker)), options);
        }

        public static IFuncWork<TInput, TOutput> Register<TInput, TOutput>(
            this IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            Func<TInput, TOutput> worker,
            RegisterOptions options = null)
        {
            return workRegistry.Register(trigger, WorkerScope.Instance(new Worker<TInput, TOutput>(worker)), options);
        }

        public static IActionWork Register<TInput>(
            this IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            IWorker worker,
            RegisterOptions options = null)
        {
            return workRegistry.Register(trigger, WorkerScope.Instance(worker), options);
        }

        public static IActionWork<TInput> Register<TInput>(
            this IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            IWorker<TInput> worker,
            RegisterOptions options = null)
        {
            return workRegistry.Register(trigger, WorkerScope.Instance(worker), options);
        }

        public static IFuncWork<TInput, TOutput> Register<TInput, TOutput>(
            this IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            IWorker<TInput, TOutput> worker,
            RegisterOptions options = null)
        {
            return workRegistry.Register(trigger, WorkerScope.Instance(worker), options);
        }
    }
}