namespace Flower.Works
{
    using System;
    using Flower.Workers;

    public interface IWork : IActivatable, ISuspendable
    {
        WorkState State { get; }
        IWorkRegistration Registration { get; }
        IObservable<ITriggeredWork> Triggered { get; }
        IObservable<IExecutableWork> Executed { get; }
        IObservable<IExecutableWork> Errored { get; }
        IObservable<IWork> Completed { get; }
        WorkerErrorBase LastError { get; }
        void Complete();
    }

    public interface IWork<out TInput> : IWork
    {
        new IWorkRegistration<TInput> Registration { get; }
    }

    public interface IActionWork : IWork<object>
    {
        new IActionWorkRegistration Registration { get; }
        new IObservable<ITriggeredActionWork> Triggered { get; }
        new IObservable<IExecutableActionWork> Executed { get; }
        new IObservable<IExecutableActionWork> Errored { get; }
        new IObservable<IActionWork> Completed { get; }
    }

    public interface IActionWork<TInput> : IWork<TInput>
    {
        new IActionWorkRegistration<TInput> Registration { get; }
        new IObservable<ITriggeredActionWork<TInput>> Triggered { get; }
        new IObservable<IExecutableActionWork<TInput>> Executed { get; }
        new IObservable<IExecutableActionWork<TInput>> Errored { get; }
        new IObservable<IActionWork<TInput>> Completed { get; }
    }

    public interface IFuncWork<TInput, TOutput> : IWork<TInput>
    {
        new IFuncWorkRegistration<TInput, TOutput> Registration { get; }
        new IObservable<ITriggeredFuncWork<TInput, TOutput>> Triggered { get; }
        new IObservable<IExecutableFuncWork<TInput, TOutput>> Executed { get; }
        new IObservable<IExecutableFuncWork<TInput, TOutput>> Errored { get; }
        new IObservable<IFuncWork<TInput, TOutput>> Completed { get; }
        IObservable<TOutput> Output { get; }
    }
}
