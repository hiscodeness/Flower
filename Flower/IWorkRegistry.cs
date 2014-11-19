using System;
using System.Collections.Generic;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public interface IWorkRegistry
    {
        WorkRegistryOptions Options { get; }

        IEnumerable<IWork> Works { get; }   
    
        IActionWork Register<TInput>(
           IObservable<TInput> trigger,
           Func<IScope<IWorker>> createWorkerScope);

        IActionWork<TInput> Register<TInput>(
           IObservable<TInput> trigger,
           Func<IScope<IWorker<TInput>>> createWorkerScope);

        IFuncWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope);
        
        void Complete(IWork work);
        void CompleteAll();
    }
}