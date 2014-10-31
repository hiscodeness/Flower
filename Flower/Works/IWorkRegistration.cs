using System;
using Flower.Workers;

namespace Flower.Works
{
    public interface IWorkRegistrationBase
    {
        IWorkRegistry WorkRegistry { get; }
        WorkRegistryOptions Options { get; }
    }

    public interface IWorkRegistrationBase<out TInput> : IWorkRegistrationBase
    {
        IObservable<TInput> Trigger { get; }
    }

    public interface IWorkRegistration : IWorkRegistrationBase<object>
    {
        IWorkerResolver WorkerResolver { get; }
    }

    public interface IWorkRegistration<TInput> : IWorkRegistrationBase<TInput>
    {
        IWorkerResolver<TInput> WorkerResolver { get; }
    }

    public interface IWorkRegistration<TInput, TOutput> : IWorkRegistrationBase<TInput>
    {
        IWorkerResolver<TInput, TOutput> WorkerResolver { get; }
    }
}