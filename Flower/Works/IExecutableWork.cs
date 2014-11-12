using System;

namespace Flower.Works
{
    public interface IExecutableWork : ITriggeredWork
    {
        ExecutableWorkState State { get; }
        void Execute();
        Exception Error { get; }
    }

    public interface IExecutableWork<out TInput> : ITriggeredWork<TInput>, IExecutableWork
    {

    }

    public interface IExecutableActionWork : ITriggeredActionWork, IExecutableWork<object>
    {
        IWorker Worker { get; }
    }

    public interface IExecutableActionWork<TInput> : ITriggeredActionWork<TInput>, IExecutableWork<TInput>
    {
        IWorker<TInput> Worker { get; }
    }

    public interface IExecutableFuncWork<TInput, TOutput> : ITriggeredFuncWork<TInput, TOutput>, IExecutableWork<TInput>
    {
        IWorker<TInput, TOutput> Worker { get; }
        TOutput Output { get; }
    }
}