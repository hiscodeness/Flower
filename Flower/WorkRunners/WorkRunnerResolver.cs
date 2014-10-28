using Flower.Works;

namespace Flower.WorkRunners
{
    internal class DefaultWorkRunnerResolver : IWorkRunnerResolver
    {
        public IWorkRunner Resolve(IWork work)
        {
            return new ImmediateWorkRunner();
        }
    }
}
