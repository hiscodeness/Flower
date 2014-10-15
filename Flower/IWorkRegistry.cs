using System;
using System.Collections.Generic;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public interface IWorkRegistry
    {
        IEnumerable<IWork> Works { get; }
        bool ActivateWorkWhenRegistered { get; }

        IWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger, IWorkerResolver<TInput, TOutput> workerResolver);

        WorkerErrorBehavior WorkerErrorBehavior { get; }
        void ActivateAllWorks();
        void SuspendAllWorks();
    }
}