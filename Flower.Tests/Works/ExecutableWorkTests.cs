using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Flower.Works;
using Xunit;

namespace Flower.Tests.Works
{
    public class ExecutableWorkTests
    {
        [Fact]
        public void WorkerErrorExceptionIsSetOnExecuted()
        {
            // Arrange
            var context = new WorkerErrorBehaviorTestContext(WorkerErrorBehavior.RaiseExecutedAndContinue);

            // Act
            context.Trigger(3,4,5);

            // Assert
            var result = context.Executed.Skip(1).Take(1).Single();
            Assert.Equal(TestWorkerIntToIntThrowOnEven.ErrorMessage, result.Error.Message);
        }

        [Fact]
        public void WorkerErrorStateIsSetOnExecuted()
        {
            // Arrange
            var context = new WorkerErrorBehaviorTestContext(WorkerErrorBehavior.RaiseExecutedAndContinue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            var result = context.Executed.Skip(1).Take(1).Single();
            Assert.Equal(ExecutableWorkState.Error, result.State);
        }

        [Fact]
        public void WorkerErrorOutputIsNotSetOnExecuted()
        {
            // Arrange
            var context = new WorkerErrorBehaviorTestContext(WorkerErrorBehavior.RaiseExecutedAndContinue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            var result = context.Executed.Skip(1).Take(1).Single();
            Assert.Equal(default(int), result.Output);
        }

        [Fact]
        public void ContinuingAfterWorkerErrorLeavesWorkActive()
        {
            // Arrange
            var context = new WorkerErrorBehaviorTestContext(WorkerErrorBehavior.RaiseExecutedAndContinue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.Active, context.Work.State);
        }

        [Fact]
        public void WorkerErrorIsIgnoredOnOutput()
        {
            // Arrange
            var context = new WorkerErrorBehaviorTestContext(WorkerErrorBehavior.RaiseExecutedAndContinue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.Active, context.Work.State);
            Assert.Equal(new[] { 3, 5 }, context.Output);
        }

        [Fact]
        public void WorkerErrorCanBeNotifiedOnExecutedBeforeCompletingWork()
        {
            // Arrange
            var context = new WorkerErrorBehaviorTestContext(WorkerErrorBehavior.RaiseExecutedAndCompleteWork);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.WorkerError, context.Work.State);
            Assert.Equal(new[] { 3 }, context.Output);
            Assert.Equal(new[] { 3, 0 }, context.Executed.Select(w => w.Output));
            Assert.Equal(ExecutableWorkState.Error, context.Executed.Last().State);
            Assert.Equal(TestWorkerIntToIntThrowOnEven.ErrorMessage, context.Executed.Last().Error.Message);
        }

        [Fact]
        public void WorkerErrorCanBeSwallowedOnExecuted()
        {
            // Arrange
            var context = new WorkerErrorBehaviorTestContext(WorkerErrorBehavior.SwallowErrorAndContinue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.Active, context.Work.State);
            Assert.Equal(new[] { 3, 5 }, context.Executed.Select(w => w.Output));
        }

        [Fact]
        public void WorkerErrorCanBeSwallowedOnOutput()
        {
            // Arrange
            var context = new WorkerErrorBehaviorTestContext(WorkerErrorBehavior.SwallowErrorAndContinue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.Active, context.Work.State);
            Assert.Equal(new[] { 3, 5 }, context.Output);
        }

        [Fact]
        public void WorkerErrorCanCompleteWork()
        {
            // Arrange
            var context = new WorkerErrorBehaviorTestContext(WorkerErrorBehavior.SwallowErrorAndCompleteWork);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.WorkerError, context.Work.State);
            Assert.Equal(new[] { 3 }, context.Executed.Select(w => w.Output));
        }
        
        [Fact]
        public void WorkerErrorCanCompleteWorkAndThrow()
        {
            // Arrange
            var context = new WorkerErrorBehaviorTestContext(WorkerErrorBehavior.CompleteWorkAndThrow);

            // Act
            Assert.Throws<ArgumentException>(() => context.Trigger(3, 4, 5));

            // Assert
            Assert.Equal(WorkState.WorkerError, context.Work.State);
            Assert.Equal(new[] { 3 }, context.Executed.Select(w => w.Output));
        }

        [Fact]
        public void WorkerErrorShownAsSuchWhenWorkCompletes()
        {
            // Arrange
            var options = new RegisterOptions(RegisterWorkBehavior.RegisterActivated, workerErrorBehavior: WorkerErrorBehavior.SwallowErrorAndCompleteWork);
            var workRegistry = new WorkRegistry(options);
            var trigger = new Subject<int>();
            var work = workRegistry.Register(trigger, new TestWorkerIntToIntThrowOnEven());
            WorkState? result = null;
            work.Executed.Subscribe(_ => { }, () => result = work.State);

            // Act
            trigger.OnNext(3);
            trigger.OnNext(4);
            trigger.OnNext(5);

            // Assert
            Assert.Equal(WorkState.WorkerError, result);
        }

        private sealed class WorkerErrorBehaviorTestContext : IDisposable
        {
            private readonly Subject<int> trigger;

            public WorkerErrorBehaviorTestContext(WorkerErrorBehavior behavior)
            {
                var options = new RegisterOptions(RegisterWorkBehavior.RegisterActivated, workerErrorBehavior: behavior);
                var workRegistry = new WorkRegistry(options);
                trigger = new Subject<int>();
                Work = workRegistry.Register(trigger, new TestWorkerIntToIntThrowOnEven());
                Executed = new List<IExecutableFuncWork<int, int>>();
                Output = new List<int>();
                Work.Executed.Subscribe(Executed.Add);
                Work.Output.Subscribe(Output.Add);
            }
            
            public IFuncWork<int, int> Work { get; private set; }
            public List<IExecutableFuncWork<int, int>> Executed { get; private set; }
            public List<int> Output { get; private set; }

            public void Trigger(params int[] values)
            {
                foreach (var value in values)
                {
                    trigger.OnNext(value);
                }
            }

            public void Dispose()
            {
                trigger.Dispose();
            }
        }
    }
}
