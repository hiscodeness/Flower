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
           IWorkerResolver workerResolver);

        IActionWork<TInput> Register<TInput>(
           IObservable<TInput> trigger,
           IWorkerResolver<TInput> workerResolver);

        IFuncWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> workerResolver);
        
        void Complete(IWork work);
        void CompleteAll();
    }
}