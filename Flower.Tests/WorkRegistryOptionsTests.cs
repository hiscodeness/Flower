using System;
using FakeItEasy;
using Flower.WorkRunners;
using Flower.Works;
using Xunit;

namespace Flower.Tests
{
    public class WorkRegistryOptionsTests
    {
        [Fact]
        public void SensibleDefaultOptions()
        {
            // Act
            var options = new RegisterOptions();

            // Assert
            Assert.Equal(RegisterWorkBehavior.RegisterActivated, options.RegisterWorkBehavior);
            Assert.Equal(TriggerErrorBehavior.CompleteWorkAndThrow, options.TriggerErrorBehavior);
            Assert.NotNull(options.WorkRunnerFactory);
            Assert.Equal(WorkerErrorBehavior.CompleteWorkAndThrow, options.WorkerErrorBehavior);
        }

        [Fact]
        public void CannotChangeWorkRunnerFactoryToNull()
        {
            // Arrange
            var options = new RegisterOptions();

            // Act
            var ex = Record.Exception(() => options = options.With((Func<IWork, IWorkRunner>)null));

            // Assert
            Assert.IsType<ArgumentNullException>(ex);
        }
        
        [Fact]
        public void CannotChangeWorkRunnerToNull()
        {
            // Arrange
            var options = new RegisterOptions();

            // Act
            var ex = Record.Exception(() => options = options.With((IWorkRunner)null));

            // Assert
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void CanChangeRegisterWorkerBehavior()
        {
            // Arrange
            var options = new RegisterOptions();

            // Act
            options = options.With(RegisterWorkBehavior.RegisterActivated);

            // Assert
            Assert.Equal(RegisterWorkBehavior.RegisterActivated, options.RegisterWorkBehavior);
        }

        [Fact]
        public void CanChangeTriggerErrorBehavior()
        {
            // Arrange
            var options = new RegisterOptions();

            // Act
            options = options.With(TriggerErrorBehavior.CompleteWork);

            // Assert
            Assert.Equal(TriggerErrorBehavior.CompleteWork, options.TriggerErrorBehavior);
        }

        [Fact]
        public void CanChangeWorkRunnerFactory()
        {
            // Arrange
            var options = new RegisterOptions();
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
            var options = new RegisterOptions();

            // Act
            options = options.With(WorkerErrorBehavior.Continue);

            // Assert
            Assert.Equal(WorkerErrorBehavior.Continue, options.WorkerErrorBehavior);
        }
    }
}
