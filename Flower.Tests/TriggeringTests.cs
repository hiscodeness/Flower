using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntSquared());
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
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntSquared());
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
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntSquared());
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
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntSquared());
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
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntSquared());
            var result = 0;
            work.Output.Subscribe(i => result = i);

            // Act
            trigger.OnNext(42);
            trigger.OnError(new Exception());

            // Assert
            Assert.Equal(TestWorkerIntSquared.WorkerFunc(42), result);
        }

        [Fact]
        public void TriggerErrorDoesntExecuteWork()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntSquared());
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
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntSquared());
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
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntSquared());
            work.Output.Subscribe(i => { });

            // Act
            trigger.OnError(new Exception());

            // Assert
            Assert.False(registry.Works.Any());
        }
    }
}