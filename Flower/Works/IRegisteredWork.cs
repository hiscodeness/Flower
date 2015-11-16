namespace Flower.Works
{
    using System;
    using Flower.Workers;

    internal interface IRegisteredWork : IWork
    {
        ITriggerEvents TriggerEvents { get; }
        void Complete(WorkState withState);
    }

    internal interface IRegisteredWork<out TInput> : IRegisteredWork, IWork<TInput>
    {
    }

    internal interface IRegisteredActionWork : IRegisteredWork<object>, IActionWork
    {
        void WorkerExecuted(IExecutableActionWork executedWork);
        void WorkerErrored(IExecutableActionWork erroredWork, Exception error);
    }

    internal interface IRegisteredActionWork<TInput> : IRegisteredWork<TInput>, IActionWork<TInput>
    {
        void WorkerExecuted(IExecutableActionWork<TInput> executedWork);
        void WorkerErrored(IExecutableActionWork<TInput> erroredWork, Exception error);
    }

    internal interface IRegisteredFuncWork<TInput, TOutput> : IRegisteredWork<TInput>, IFuncWork<TInput, TOutput>
    {
        void WorkerExecuted(IExecutableFuncWork<TInput, TOutput> executedWork);
        void WorkerErrored(IExecutableFuncWork<TInput, TOutput> erroredWork, Exception error);
    }
}
