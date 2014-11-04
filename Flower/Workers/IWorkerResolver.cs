using System;

namespace Flower.Workers
{
    public interface IWorkerResolver
    {
        void Release(IWorker worker);
        IWorker Resolve();
    }
    
    public interface IWorkerResolver<TInput>
    {
        void Release(IWorker<TInput> worker);
        IWorker<TInput> Resolve(TInput input);
    }

    public interface IWorkerResolver<TInput, TOutput>
    {
        void Release(IWorker<TInput, TOutput> worker);
        IWorker<TInput, TOutput> Resolve(TInput input);
    }
}