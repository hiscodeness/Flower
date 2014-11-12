using Flower.WorkRunners;

namespace Flower.Works
{
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

    public interface ITriggeredActionWork<TInput> : ITriggeredWork<TInput>
    {
        new IActionWork<TInput> Work { get; }
    }

    public interface ITriggeredFuncWork<TInput, TOutput> : ITriggeredWork<TInput>
    {
        new IFuncWork<TInput, TOutput> Work { get; }
    }
}