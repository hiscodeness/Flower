using Flower.Works;

namespace Flower.WorkRunners
{
    public interface IWorkRunnerResolver
    {
        IWorkRunner Resolve(IWork work);
    }
}
