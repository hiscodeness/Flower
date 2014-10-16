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

        WorkerErrorBehavior WorkerErrorBehavior { get; }
        void ActivateAllWorks();

        IWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger, IWorkerResolver<TInput, TOutput> workerResolver);

        void SuspendAllWorks();
    }
}