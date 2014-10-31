using Flower.WorkRunners;
using Flower.Works;

namespace Flower
{
    internal interface IRegisteredWork<TInput> : IWorkBase<TInput>
    {
        ITriggeredWorkBase Trigger(IWorkRunner workRunner, TInput input);
    }
}