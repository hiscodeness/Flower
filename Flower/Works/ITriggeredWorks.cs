using Flower.WorkRunners;

namespace Flower.Works
{
    /// <summary>
    /// Work that has been triggered and submitted to a <see cref="IWorkRunner" /> for execution.
    /// </summary>
    public interface ITriggeredWork
    {
        IWork Work { get; }
        IWorkRunner WorkRunner { get; }
    }

    public interface ITriggeredWork<out TInput> : ITriggeredWork
    {
        TInput Input { get; }
        new IWork<TInput> Work { get; }
    }

    public interface ITriggeredActionWork : ITriggeredWork<object>
    {
    }

    /// <summary>
    /// Work that has been triggered and submitted to a <see cref="IWorkRunner" />.
    /// </summary>
    public interface ITriggeredActionWork<TInput> : ITriggeredWork<TInput>
    {
        new IActionWork<TInput> Work { get; }
    }

    /// <summary>
    /// Work that has been triggered and submitted to a <see cref="IWorkRunner" />.
    /// </summary>
    public interface ITriggeredFuncWork<TInput, TOutput> : ITriggeredWork<TInput>
    {
        new IFuncWork<TInput, TOutput> Work { get; }
    }
}