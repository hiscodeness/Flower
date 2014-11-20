using System;
using System.Collections.Generic;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public interface IWorkRegistry
    {
        WorkRegistryOptions DefaultOptions { get; }

        IEnumerable<IWork> Works { get; }

        IActionWork Register<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker>> createWorkerScope,
            WorkRegistryOptions options = null);

        IActionWork<TInput> Register<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput>>> createWorkerScope,
            WorkRegistryOptions options = null);

        IFuncWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope,
            WorkRegistryOptions options = null);

        void Complete(IWork work);
        void CompleteAll();
    }
}