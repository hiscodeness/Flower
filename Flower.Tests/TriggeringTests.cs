using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Flower.Works;
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
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
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
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
            var result = 0;
            work.Output.Subscribe(i => result = i);

            // Act
            trigger.OnNext(42);
            trigger.OnCompleted();

            // Assert
            Assert.Equal(TestWorkerIntToIntSquared.WorkerFunc(42), result);
        }

        [Fact]
        public void TriggerCompletedSetsWorkStateToComplete()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
            work.Output.SingleOrDefaultAsync().Subscribe(i => { });

            // Act
            trigger.OnCompleted();

            // Assert
            Assert.Equal(WorkState.Completed, work.State);
        }

        [Fact]
        public void TriggerCompletedCompletesWork()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
            work.Output.SingleOrDefaultAsync().Subscribe(i => { });

            // Act
            trigger.OnCompleted();

            // Assert
            Assert.False(registry.Works.Any());
        }

        [Fact]
        public void WorkIsNotTriggeredOnTriggerError()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
            ITriggeredFuncWork<int, int> result = null;
            work.Triggered.Subscribe(w => result = w, _ => { });

            // Act
            trigger.OnError(new Exception());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TriggerErrorAllowsLastOutputToComeThrough()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
            var result = 0;
            work.Output.Subscribe(i => result = i, _ => { });

            // Act
            trigger.OnNext(42);
            trigger.OnError(new Exception());

            // Assert
            Assert.Equal(TestWorkerIntToIntSquared.WorkerFunc(42), result);
        }

        [Fact]
        public void TriggerErrorsThrowIfSubscribersDoNotHandleThem()
        {
            // Arrange
            var options = new RegisterOptions();
            var registry = new WorkRegistry(options);
            var trigger = new Subject<int>();
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
            // Output subscription uses an overload that does not specify a delegate for the OnError notification
            work.Output.Subscribe(_ => { });

            // Act
            var ex = Record.Exception(() => trigger.OnError(new Exception()));

            // Assert
            Assert.IsType<Exception>(ex);
        }
        
        [Fact]
        public void TriggerErrorsAreForwardedToOutputSubscribers()
        {
            // Arrange
            var trigger = new Subject<int>();
            var options = new RegisterOptions();
            var registry = new WorkRegistry(options);
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
            Exception exception = null;
            work.Output.Subscribe(_ => { }, ex => exception = ex);
            var expected = new Exception();

            // Act
            trigger.OnError(expected);

            // Assert
            Assert.Equal(expected, exception);
        }

        [Fact]
        public void TriggerErrorsCanBeSwallowed()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry =
                new WorkRegistry(
                    new RegisterOptions(
                        RegisterWorkBehavior.RegisterActivated, TriggerErrorBehavior.SwallowErrorAndCompleteWork));
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
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
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
            int? result = null;
            work.Output.Subscribe(i => result = i, _ => {});

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
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
            work.Output.Subscribe(i => { }, _ => {});

            // Act
            trigger.OnError(new Exception());

            // Assert
            Assert.Equal(WorkState.TriggerError, work.State);
        }

        [Fact]
        public void TriggerErrorCompletesWork()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
            work.Output.Subscribe(i => { }, _ => {});

            // Act
            trigger.OnError(new Exception());

            // Assert
            Assert.False(registry.Works.Any());
        }
    }
}