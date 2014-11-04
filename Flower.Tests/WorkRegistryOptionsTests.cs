using System;
using FakeItEasy;
using Flower.WorkRunners;
using Xunit;

namespace Flower.Tests
{
    public class WorkRegistryOptionsTests
    {
        [Fact]
        public void SensibleDefaultOptions()
        {
            // Act
            var options = new WorkRegistryOptions();

            // Assert
            Assert.Equal(RegisterWorkBehavior.RegisterSuspended, options.RegisterWorkBehavior);
            Assert.Equal(TriggerErrorBehavior.CompleteWorkAndForwardError, options.TriggerErrorBehavior);
            Assert.NotNull(options.WorkRunnerResolver);
            Assert.Equal(WorkerErrorBehavior.CompleteWorkAndThrow, options.WorkerErrorBehavior);
        }

        [Fact]
        public void CannotChangeWorkRunnerResolverToNull()
        {
            // Arrange
            var options = new WorkRegistryOptions();

            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => options = options.With(null));
        }


        [Fact]
        public void CanChangeRegisterWorkerBehavior()
        {
            // Arrange
            var options = new WorkRegistryOptions();

            // Act
            options = options.With(RegisterWorkBehavior.RegisterActivated);

            // Assert
            Assert.Equal(RegisterWorkBehavior.RegisterActivated, options.RegisterWorkBehavior);
        }

        [Fact]
        public void CanChangeTriggerErrorBehavior()
        {
            // Arrange
            var options = new WorkRegistryOptions();

            // Act
            options = options.With(TriggerErrorBehavior.CompleteWork);

            // Assert
            Assert.Equal(TriggerErrorBehavior.CompleteWork, options.TriggerErrorBehavior);
        }

        [Fact]
        public void CanChangeWorkRunnerResolver()
        {
            // Arrange
            var options = new WorkRegistryOptions();
            var workRunnerResolver = A.Fake<IWorkRunnerResolver>();

            // Act
            options = options.With(workRunnerResolver);

            // Assert
            Assert.Equal(workRunnerResolver, options.WorkRunnerResolver);
        }

        [Fact]
        public void CanChangeWorkerErrorBehavior()
        {
            // Arrange
            var options = new WorkRegistryOptions();

            // Act
            options = options.With(WorkerErrorBehavior.Ignore);

            // Assert
            Assert.Equal(WorkerErrorBehavior.Ignore, options.WorkerErrorBehavior);
        }
    }
}
