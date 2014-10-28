using Flower.Works;

namespace Flower.WorkRunners
{
    internal class DefaultWorkRunnerResolver : IWorkRunnerResolver
    {
        public IWorkRunner Resolve(IWorkBase work)
        {
            return new ImmediateWorkRunner();
        }
    }
}
