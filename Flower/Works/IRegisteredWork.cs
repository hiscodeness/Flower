namespace Flower.Works
{
    using Flower.Workers;

    internal interface IRegisteredWork : IWork
    {
        ITriggerEvents TriggerEvents { get; }
        void SetWorkerError(WorkerError workerError);
        void Complete(WorkState withState);
    }

    internal interface IRegisteredWork<out TInput> : IRegisteredWork, IWork<TInput>
    {
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