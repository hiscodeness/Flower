using System;

namespace Flower.Works
{
    using System.Threading.Tasks;

    public interface IExecutableWork : ITriggeredWork
    {
        ExecutableWorkState State { get; }
        Task Execute();
        Exception Error { get; }
    }

    public interface IExecutableWork<out TInput> : ITriggeredWork<TInput>, IExecutableWork
    {

    }

    public interface IExecutableActionWork : ITriggeredActionWork, IExecutableWork<object>
    {
        IScope<IWorker> WorkerScope { get; }
    }

    public interface IExecutableActionWork<TInput> : ITriggeredActionWork<TInput>, IExecutableWork<TInput>
    {
        IScope<IWorker<TInput>> WorkerScope { get; }
    }

    public interface IExecutableFuncWork<TInput, TOutput> : ITriggeredFuncWork<TInput, TOutput>, IExecutableWork<TInput>
    {
        IScope<IWorker<TInput, TOutput>> WorkerScope { get; }
        TOutput Output { get; }
    }
}