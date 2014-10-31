using Flower.WorkRunners;

namespace Flower.Works
{
    /// <summary>
    /// Work that has been triggered and submitted to a <see cref="IWorkRunner" />.
    /// </summary>
    public interface ITriggeredWorkBase
    {
        TriggeredWorkState State { get; }
        IWorkBase Work { get; }
        IWorkRunner WorkRunner { get; }
        void Submit();
        void Execute();
    }

    public interface ITriggeredWork : ITriggeredWorkBase
    {
        IWorker Worker { get; }
    }

    /// <summary>
    /// Work that has been triggered and submitted to a <see cref="IWorkRunner" />.
    /// </summary>
    public interface ITriggeredWork<TInput> : ITriggeredWorkBase
    {
        new IWork<TInput> Work { get; }
        IWorker<TInput> Worker { get; }
        TInput Input { get; }
    }

    /// <summary>
    /// Work that has been triggered and submitted to a <see cref="IWorkRunner" />.
    /// </summary>
    public interface ITriggeredWork<TInput, TOutput> : ITriggeredWorkBase
    {
        new IWork<TInput, TOutput> Work { get; }
        IWorker<TInput, TOutput> Worker { get; }
        TInput Input { get; }
        TOutput Output { get; }
    }
}