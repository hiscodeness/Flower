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
    }
}
