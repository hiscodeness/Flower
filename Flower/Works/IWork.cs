using System;

namespace Flower.Works
{
    public interface IWorkBase: IActivatable, ISuspendable, IUnregistrable
    {
        WorkState State { get; }
        IWorkRegistrationBase Registration { get; }
    }

    public interface IWork : IWorkBase
    {
        new IWorkRegistration Registration { get; }
        IObservable<ITriggeredWork> Triggered { get; }
        IObservable<ITriggeredWork> Executed { get; }
    }

    public interface IWork<TInput> : IWorkBase
    {
        new IWorkRegistration<TInput> Registration { get; }
        IObservable<ITriggeredWork<TInput>> Triggered { get; }
        IObservable<ITriggeredWork<TInput>> Executed { get; }
    }

    public interface IWork<TInput, TOutput> : IWorkBase
    {
        new IWorkRegistration<TInput, TOutput> Registration { get; }
        IObservable<ITriggeredWork<TInput, TOutput>> Triggered { get; }
        IObservable<ITriggeredWork<TInput, TOutput>> Executed { get; }
        IObservable<TOutput> Output { get; }
    }
}