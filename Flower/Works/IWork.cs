using System;
using Flower.Workers;

namespace Flower.Works
{
    public interface IWork : IActivatable, ISuspendable
    {
        IWorkRegistry WorkRegistry { get; }
        WorkState State { get; }
        IWorkerResolver WorkerResolver { get; }
        IObservable<object> Trigger { get; }
        IObservable<ITriggeredWorkBase> Executed { get; }
    }

    public interface IWork<TInput> : IWork
    {
        new IObservable<TInput> Trigger { get; }
        new IWorkerResolver<TInput> WorkerResolver { get; }
        new IObservable<ITriggeredWork<TInput>> Executed { get; }
    }

    public interface IWork<TInput, TOutput> : IWork<TInput>
    {
        new IWorkerResolver<TInput, TOutput> WorkerResolver { get; }
        new IObservable<ITriggeredWork<TInput, TOutput>> Executed { get; }
        IObservable<TOutput> Output { get; }
        IObservable<ITriggeredWork<TInput,TOutput>> Triggered { get; }
    }
}