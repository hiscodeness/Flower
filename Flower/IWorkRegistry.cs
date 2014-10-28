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

        IWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> workerResolver);

        void Unregister(IWork work);
        
        void ActivateAllWorks();

        void SuspendAllWorks();
    }
}