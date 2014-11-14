using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal interface IRegisteredWork : IWork
    {
        ITriggerEvents TriggerEvents { get; }
        void Complete(WorkState withState);
    }

    internal interface IRegisteredWork<TInput> : IRegisteredWork, IWork<TInput>
    {
        IExecutableWork Trigger(IWorkRunner workRunner, TInput input);
    }

    internal interface IRegisteredActionWork : IRegisteredWork<object>, IActionWork
    {
        void WorkerExecuted(IExecutableActionWork triggeredWork);
    }

    internal interface IRegisteredActionWork<TInput> : IRegisteredWork<TInput>, IActionWork<TInput>
    {
        void WorkerExecuted(IExecutableActionWork<TInput> triggeredWork);
    }

    internal interface IRegisteredFuncWork<TInput, TOutput> : IRegisteredWork<TInput>, IFuncWork<TInput, TOutput>
    {
        void WorkerExecuted(IExecutableFuncWork<TInput, TOutput> triggeredWork);
    }
}