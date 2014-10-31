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

    public interface ITriggeredWorkBase<out TInput> : ITriggeredWorkBase
    {
        TInput Input { get; }
        new IWorkBase<TInput> Work { get; }
    }

    public interface ITriggeredWork : ITriggeredWorkBase<object>
    {
        IWorker Worker { get; }
    }

    /// <summary>
    /// Work that has been triggered and submitted to a <see cref="IWorkRunner" />.
    /// </summary>
    public interface ITriggeredWork<TInput> : ITriggeredWorkBase<TInput>
    {
        new IWork<TInput> Work { get; }
        IWorker<TInput> Worker { get; }
    }

    /// <summary>
    /// Work that has been triggered and submitted to a <see cref="IWorkRunner" />.
    /// </summary>
    public interface ITriggeredWork<TInput, TOutput> : ITriggeredWorkBase<TInput>
    {
        new IWork<TInput, TOutput> Work { get; }
        IWorker<TInput, TOutput> Worker { get; }
        TOutput Output { get; }
    }
}