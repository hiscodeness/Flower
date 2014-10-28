using System;
using Flower.Workers;

namespace Flower.Works
{
    public interface IWorkRegistrationBase : IDisposable
    {
        IWorkRegistry WorkRegistry { get; }
    }

    public interface IWorkRegistration : IWorkRegistrationBase
    {
        IObservable<object> Trigger { get; }
        IWorkerResolver WorkerResolver { get; }
    }

    public interface IWorkRegistration<TInput> : IWorkRegistrationBase
    {
        IObservable<TInput> Trigger { get; }
        IWorkerResolver<TInput> WorkerResolver { get; }
    }

    public interface IWorkRegistration<TInput, TOutput> : IWorkRegistrationBase
    {
        IObservable<TInput> Trigger { get; }
        IWorkerResolver<TInput, TOutput> WorkerResolver { get; }
    }
}