using Flower.Tests.TestDoubles;
using Xunit;

namespace Flower.Tests
{
    public class WorkTests
    {
        [Fact]
        public void PipingWorkOutputToWorkerInputSucceeds()
        {
            // Arrange
            var work = TestWorks.IntSquaredWork;
            var worker = TestWorkers.IntSquaredWorker;

            // Act
            var pipedWork = work.Pipe(worker);

            // Assert
            Assert.NotNull(pipedWork.Trigger);
            Assert.Equal(work.Output, pipedWork.Trigger);
        }
        
        [Fact]
        public void PipingWorkOutputToWorkerResolverInputSucceeds()
        {
            // Arrange
            var work = TestWorks.IntSquaredWork;
            var workerResolver = TestWorkerResolvers.IntSquaredWorkerResolver;

            // Act
            var pipedWork = work.Pipe(workerResolver);

            // Assert
            Assert.NotNull(pipedWork.Trigger);
            Assert.Equal(work.Output, pipedWork.Trigger);
        }
    }
}
