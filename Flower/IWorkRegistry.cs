using System;
using System.Collections.Generic;
using Flower.Works;

namespace Flower
{
    public interface IWorkRegistry
    {
        RegisterOptions DefaultOptions { get; }

        IEnumerable<IWork> Works { get; }

        IActionWork Register<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker>> createWorkerScope,
            RegisterOptions options = null);

        IActionWork<TInput> Register<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput>>> createWorkerScope,
            RegisterOptions options = null);

        IFuncWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope,
            RegisterOptions options = null);

        void Complete(IWork work);
        void CompleteAll();
    }
}