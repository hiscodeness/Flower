namespace Flower.Tests
{
    using System;
    using FakeItEasy;
    using Flower.WorkRunners;
    using Flower.Works;
    using Xunit;

    public class WorkRegistryOptionsTests
    {
        [Fact]
        public void SensibleDefaultOptions()
        {
            // Act
            var options = new WorkOptions();

            // Assert
            Assert.Equal(WorkRegisterMode.Activated, options.WorkRegisterMode);
            Assert.Equal(TriggerErrorMode.ErrorWork, options.TriggerErrorMode);
            Assert.Equal(WorkerErrorMode.Continue, options.WorkerErrorMode);
            Assert.NotNull(options.WorkRunnerFactory);
            Assert.Null(options.WorkDecoratorFactory);
        }

        [Fact]
        public void CannotChangeWorkRunnerFactoryToNull()
        {
            // Arrange
            var options = new WorkOptions();

            // Act
            var ex = Record.Exception(() => options = options.With((Func<IWork, IWorkRunner>) null));

            // Assert
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void CannotChangeWorkRunnerToNull()
        {
            // Arrange
            var options = new WorkOptions();

            // Act
            var ex = Record.Exception(() => options = options.With((IWorkRunner) null));

            // Assert
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void CanChangeRegisterWorkerBehavior()
        {
            // Arrange
            var options = new WorkOptions();

            // Act
            options = options.With(WorkRegisterMode.Activated);

            // Assert
            Assert.Equal(WorkRegisterMode.Activated, options.WorkRegisterMode);
        }

        [Fact]
        public void CanChangeTriggerErrorBehavior()
        {
            // Arrange
            var options = new WorkOptions();

            // Act
            options = options.With(TriggerErrorMode.CompleteWork);

            // Assert
            Assert.Equal(TriggerErrorMode.CompleteWork, options.TriggerErrorMode);
        }

        [Fact]
        public void CanChangeWorkRunnerFactory()
        {
            // Arrange
            var options = new WorkOptions();
            var workRunner = A.Fake<IWorkRunner>();
            Func<IWork, IWorkRunner> workRunnerFactory = _ => workRunner;

            // Act
            options = options.With(workRunnerFactory);

            // Assert
            Assert.Equal(workRunnerFactory, options.WorkRunnerFactory);
        }

        [Fact]
        public void CanChangeWorkerErrorBehavior()
        {
            // Arrange
            var options = new WorkOptions();

            // Act
            options = options.With(WorkerErrorMode.Continue);

            // Assert
            Assert.Equal(WorkerErrorMode.Continue, options.WorkerErrorMode);
        }
    }
}
