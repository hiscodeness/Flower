using System;
using Flower.Workers;

namespace Flower.Works
{
    public interface IWorkBase: IActivatable, ISuspendable, IUnregistrable
    {
        WorkState State { get; }        
    }

    public interface IWork : IWorkBase
    {
        IWorkRegistration Registration { get; }
        IObservable<ITriggeredWork> Triggered { get; }
        IObservable<ITriggeredWork> Executed { get; }
    }

    public interface IWork<TInput> : IWorkBase
    {
        IWorkRegistration<TInput> Registration { get; }
        IObservable<ITriggeredWork<TInput>> Triggered { get; }
        IObservable<ITriggeredWork<TInput>> Executed { get; }
    }

    public interface IWork<TInput, TOutput> : IWorkBase
    {
        IWorkRegistration<TInput, TOutput> Registration { get; }
        IObservable<ITriggeredWork<TInput, TOutput>> Triggered { get; }
        IObservable<ITriggeredWork<TInput, TOutput>> Executed { get; }
        IObservable<TOutput> Output { get; }
    }
}