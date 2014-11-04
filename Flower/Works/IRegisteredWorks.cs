using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal interface IRegisteredWork : IWork
    {
        ITriggerEvents TriggerEvents { get; }
        void WorkerErrored(Exception error);
    }

    internal interface IRegisteredWork<TInput> : IRegisteredWork, IWork<TInput>
    {
        ITriggeredWork Trigger(IWorkRunner workRunner, TInput input);
    }

    internal interface IRegisteredActionWork : IRegisteredWork<object>, IActionWork
    {
        void WorkerExecuted(ITriggeredActionWork triggeredWork);
    }

    internal interface IRegisteredActionWork<TInput> : IRegisteredWork<TInput>, IActionWork<TInput>
    {
        void WorkerExecuted(ITriggeredActionWork<TInput> triggeredWork);
    }

    internal interface IRegisteredFuncWork<TInput, TOutput> : IRegisteredWork<TInput>, IFuncWork<TInput, TOutput>
    {
        void WorkerExecuted(ITriggeredFuncWork<TInput, TOutput> triggeredWork);
    }
}