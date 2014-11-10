using System;

namespace Flower.Works
{
    public interface IWork: IActivatable, ISuspendable, IUnregistrable
    {
        WorkState State { get; }
        IWorkRegistration Registration { get; }
    }

    public interface IWork<out TInput> : IWork
    {
        new IWorkRegistration<TInput> Registration { get; }
    }

    public interface IActionWork : IWork<object>
    {
        new IActionWorkRegistration Registration { get; }
        IObservable<ITriggeredActionWork> Triggered { get; }
        IObservable<IExecutableActionWork> Executed { get; }
    }

    public interface IActionWork<TInput> : IWork<TInput>
    {
        new IActionWorkRegistration<TInput> Registration { get; }
        IObservable<ITriggeredActionWork<TInput>> Triggered { get; }
        IObservable<IExecutableActionWork<TInput>> Executed { get; }
    }

    public interface IFuncWork<TInput, TOutput> : IWork<TInput>
    {
        new IFuncWorkRegistration<TInput, TOutput> Registration { get; }
        IObservable<ITriggeredFuncWork<TInput, TOutput>> Triggered { get; }
        IObservable<IExecutableFuncWork<TInput, TOutput>> Executed { get; }
        IObservable<TOutput> Output { get; }
    }
}