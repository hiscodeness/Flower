using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Xunit;

namespace Flower.Tests
{
    public class TriggeringTests
    {
        [Fact]
        public void TriggerCompletedDoesntExecuteWork()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = WorkRegistryFactory.CreateAutoActivating();
            var work = registry.Register(trigger, TestWorkers.IntSquaredWorker);
            int? result = null;
            work.Output.Subscribe(i => result = i);

            // Act
            trigger.OnCompleted();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TriggerCompletedLetsLastOutputToComeThrough()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = WorkRegistryFactory.CreateAutoActivating();
            var work = registry.Register(trigger, TestWorkers.IntSquaredWorker);
            var result = 0;
            work.Output.Subscribe(i => result = i);

            // Act
            trigger.OnNext(42);
            trigger.OnCompleted();

            // Assert
            Assert.Equal(TestWorkerIntSquared.WorkerFunc(42), result);
        }

        [Fact]
        public void TriggerCompletedSetsWorkStateToComplete()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = WorkRegistryFactory.CreateAutoActivating();
            var work = registry.Register(trigger, TestWorkers.IntSquaredWorker);
            work.Output.SingleOrDefaultAsync().Subscribe(i => { });

            // Act
            trigger.OnCompleted();

            // Assert
            Assert.Equal(WorkState.Completed, work.State);
        }

        [Fact]
        public void TriggerCompletedUnregistersWork()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = WorkRegistryFactory.CreateAutoActivating();
            var work = registry.Register(trigger, TestWorkers.IntSquaredWorker);
            work.Output.SingleOrDefaultAsync().Subscribe(i => { });

            // Act
            trigger.OnCompleted();

            // Assert
            Assert.False(registry.Works.Any());
        }

        [Fact]
        public void TriggerErrorAllowsLastOutputToComeThrough()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = WorkRegistryFactory.CreateAutoActivating();
            var work = registry.Register(trigger, TestWorkers.IntSquaredWorker);
            var result = 0;
            work.Output.Subscribe(i => result = i);

            // Act
            trigger.OnNext(42);
            trigger.OnError(new Exception());

            // Assert
            Assert.Equal(TestWorkerIntSquared.WorkerFunc(42), result);
        }

        [Fact]
        public void TriggerErrorsThrowIfSubscribersDoNotHandleThem()
        {
            // Arrange
            var options = new WorkRegistryOptions(RegisterWorkBehavior.RegisterActivated);
            var registry = new WorkRegistry(options);
            var trigger = new Subject<int>();
            var work = registry.Register(trigger, TestWorkers.IntSquaredWorker);
            // * Output subscription uses an overload that does not specify a delegate for the OnError notification
            work.Output.Subscribe(_ => { });

            // Act / Assert
            // Should throw here because of * described above
            Assert.Throws<Exception>(() => trigger.OnError(new Exception()));
        }
        
        [Fact]
        public void TriggerErrorsAreForwardedToOutputSubscribers()
        {
            // Arrange
            var trigger = new Subject<int>();
            var options = new WorkRegistryOptions(RegisterWorkBehavior.RegisterActivated);
            var registry = new WorkRegistry(options); 
            var work = registry.Register(trigger, TestWorkers.IntSquaredWorker);
            Exception exception = null;
            work.Output.Subscribe(_ => { }, ex => exception = ex);

            // Act
            var expected = new Exception();
            trigger.OnError(expected);

            // Assert
            Assert.Equal(expected, exception);
        }

        [Fact]
        public void TriggerErrorsAreNotForwardedToOutputSubscribers()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = WorkRegistryFactory.CreateAutoActivating();
            var work = registry.Register(trigger, TestWorkers.IntSquaredWorker);
            Exception exception = null;
            work.Output.Subscribe(_ => { }, ex => exception = ex);

            // Act
            trigger.OnError(new Exception());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void TriggerErrorDoesntExecuteWork()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = WorkRegistryFactory.CreateAutoActivating();
            var work = registry.Register(trigger, TestWorkers.IntSquaredWorker);
            int? result = null;
            work.Output.Subscribe(i => result = i);

            // Act
            trigger.OnError(new Exception());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TriggerErrorSetsWorkStateToTriggerError()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = WorkRegistryFactory.CreateAutoActivating();
            var work = registry.Register(trigger, TestWorkers.IntSquaredWorker);
            work.Output.Subscribe(i => { });

            // Act
            trigger.OnError(new Exception());

            // Assert
            Assert.Equal(WorkState.TriggerError, work.State);
        }

        [Fact]
        public void TriggerErrorUnregistersWork()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = WorkRegistryFactory.CreateAutoActivating();
            var work = registry.Register(trigger, TestWorkers.IntSquaredWorker);
            work.Output.Subscribe(i => { });

            // Act
            trigger.OnError(new Exception());

            // Assert
            Assert.False(registry.Works.Any());
        }
    }
}