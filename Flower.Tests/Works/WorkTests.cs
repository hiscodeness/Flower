using System;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Xunit;

namespace Flower.Tests.Works
{
    public class WorkTests
    {
        [Fact]
        public void UnregisteredWorkCannotBeActivated()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var work = workRegistry.Register(subject, TestWorkers.IntSquaredWorker);
            workRegistry.Unregister(work);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => work.Activate());
        }

        [Fact]
        public void PipingWorkOutputToWorkerInputSucceeds()
        {
            // Arrange
            var work = TestWorks.IntSquaredWork;
            var worker = TestWorkers.IntSquaredWorker;

            // Act
            var pipedWork = work.Pipe(worker);

            // Assert
            Assert.NotNull(pipedWork.Registration.Trigger);
            Assert.Equal(work.Output, pipedWork.Registration.Trigger);
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
            Assert.NotNull(pipedWork.Registration.Trigger);
            Assert.Equal(work.Output, pipedWork.Registration.Trigger);
        }
    }
}