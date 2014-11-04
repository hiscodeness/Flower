using Flower.WorkRunners;

namespace Flower.Works
{
    /// <summary>
    /// Work that has been triggered and submitted to a <see cref="IWorkRunner" /> for execution.
    /// </summary>
    public interface ITriggeredWork
    {
        TriggeredWorkState State { get; }
        IWork Work { get; }
        IWorkRunner WorkRunner { get; }
        void Execute();
    }

    public interface ITriggeredWork<out TInput> : ITriggeredWork
    {
        TInput Input { get; }
        new IWork<TInput> Work { get; }
    }

    public interface ITriggeredActionWork : ITriggeredWork<object>
    {
        IWorker Worker { get; }
    }

    /// <summary>
    /// Work that has been triggered and submitted to a <see cref="IWorkRunner" />.
    /// </summary>
    public interface ITriggeredActionWork<TInput> : ITriggeredWork<TInput>
    {
        new IActionWork<TInput> Work { get; }
        IWorker<TInput> Worker { get; }
    }

    /// <summary>
    /// Work that has been triggered and submitted to a <see cref="IWorkRunner" />.
    /// </summary>
    public interface ITriggeredFuncWork<TInput, TOutput> : ITriggeredWork<TInput>
    {
        new IFuncWork<TInput, TOutput> Work { get; }
        IWorker<TInput, TOutput> Worker { get; }
        TOutput Output { get; }
    }
}