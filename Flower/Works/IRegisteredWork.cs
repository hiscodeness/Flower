using System;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal interface IRegisteredWorkBase<in TInput>
    {
        ITriggeredWorkBase Trigger(IWorkRunner workRunner, TInput input);
    }

    internal interface IRegisteredWork : IRegisteredWorkBase<object>, IWork
    {
        void WorkerErrored(ITriggeredWork triggeredWork, Exception error);
        void WorkerExecuted(ITriggeredWork triggeredWork);
    }

    internal interface IRegisteredWork<TInput> : IRegisteredWorkBase<TInput>, IWork<TInput>
    {
        void WorkerErrored(ITriggeredWork<TInput> triggeredWork, Exception error);
        void WorkerExecuted(ITriggeredWork<TInput> triggeredWork);
    }

    internal interface IRegisteredWork<TInput, TOutput> : IRegisteredWorkBase<TInput>, IWork<TInput, TOutput>
    {
        void WorkerErrored(ITriggeredWork<TInput, TOutput> triggeredWork, Exception error);
        void WorkerExecuted(ITriggeredWork<TInput, TOutput> triggeredWork);
    }
}