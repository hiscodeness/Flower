using System;
using Flower.Workers;

namespace Flower.Works
{
    public interface IWorkRegistration
    {
        IWorkRegistry WorkRegistry { get; }
        WorkRegistryOptions Options { get; }
    }

    public interface IWorkRegistration<out TInput> : IWorkRegistration
    {
        IObservable<TInput> Trigger { get; }
    }

    public interface IActionWorkRegistration : IWorkRegistration<object>
    {
        IWorkerResolver WorkerResolver { get; }
    }

    public interface IActionWorkRegistration<TInput> : IWorkRegistration<TInput>
    {
        IWorkerResolver<TInput> WorkerResolver { get; }
    }

    public interface IFuncWorkRegistration<TInput, TOutput> : IWorkRegistration<TInput>
    {
        IWorkerResolver<TInput, TOutput> WorkerResolver { get; }
    }
}