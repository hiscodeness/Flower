using Flower.Workers;

namespace Flower.Tests.TestDoubles
{
    internal class TestWorkerResolvers
    {
        internal static readonly IWorkerResolver<int, int> IntSquaredWorkerResolver =
            WorkerResolver.CreateFromInstance(TestWorkers.IntSquaredWorker);
    }
}