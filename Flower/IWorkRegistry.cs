using System;
using System.Collections.Generic;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public interface IWorkRegistry : IDisposable
    {
        WorkRegistryOptions Options { get; }

        IEnumerable<IWorkBase> Works { get; }

        IWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> workerResolver);

        void Unregister(IWorkBase work);
        
        void ActivateAllWorks();

        void SuspendAllWorks();
    }
}