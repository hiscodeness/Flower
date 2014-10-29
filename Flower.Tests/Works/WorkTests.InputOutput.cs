using System;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Xunit;

namespace Flower.Tests.Works
{
    public partial class WorkTests
    {
        [Fact]
        public void UnregisteredWorkCannotBeActivated()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var work = workRegistry.Register(subject, new TestWorkerIntToIntSquared());
            workRegistry.Unregister(work);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => work.Activate());
        }

        [Fact]
        public void PipingWorkOutputToWorkerInputSucceeds()
        {
            // Arrange
            var work = new TestWorkIntToIntSquared();
            var worker = new TestWorkerIntToIntSquared();

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
            var work = new TestWorkIntToIntSquared();
            var workerResolver = new TestWorkerIntToIntSquared();

            // Act
            var pipedWork = work.Pipe(workerResolver);

            // Assert
            Assert.NotNull(pipedWork.Registration.Trigger);
            Assert.Equal(work.Output, pipedWork.Registration.Trigger);
        }
    }
}