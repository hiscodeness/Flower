using Flower.Workers;

namespace Flower.Tests.TestDoubles
{
    internal static class TestWorkerResolvers
    {
        internal static readonly IWorkerResolver<int, int> IntSquaredWorkerResolver =
            WorkerResolver.CreateFromInstance(TestWorkers.IntSquaredWorker);
    }
}