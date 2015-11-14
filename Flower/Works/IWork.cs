using System;

namespace Flower.Works
{
    using Flower.Workers;

    public interface IWork: IActivatable, ISuspendable
    {
        WorkState State { get; }
        IWorkRegistration Registration { get; }
        IObservable<IExecutableWork> Executed { get; }
        IObservable<IWork> Completed { get; }
        WorkerError LastError { get; }
        void Complete();
    }

    public interface IWork<out TInput> : IWork
    {
        new IWorkRegistration<TInput> Registration { get; }
    }

    public interface IActionWork : IWork<object>
    {
        new IActionWorkRegistration Registration { get; }
        IObservable<ITriggeredActionWork> Triggered { get; }
        new IObservable<IExecutableActionWork> Executed { get; }
        new IObservable<IActionWork> Completed { get; }
    }

    public interface IActionWork<TInput> : IWork<TInput>
    {
        new IActionWorkRegistration<TInput> Registration { get; }
        IObservable<ITriggeredActionWork<TInput>> Triggered { get; }
        new IObservable<IExecutableActionWork<TInput>> Executed { get; }
        new IObservable<IActionWork<TInput>> Completed { get; }
    }

    public interface IFuncWork<TInput, TOutput> : IWork<TInput>
    {
        new IFuncWorkRegistration<TInput, TOutput> Registration { get; }
        IObservable<ITriggeredFuncWork<TInput, TOutput>> Triggered { get; }
        new IObservable<IExecutableFuncWork<TInput, TOutput>> Executed { get; }
        new IObservable<IFuncWork<TInput, TOutput>> Completed { get; }
        IObservable<TOutput> Output { get; }
    }
}
