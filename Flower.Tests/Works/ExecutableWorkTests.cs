namespace Flower.Tests.Works
{
    using System;
    using System.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using Flower.Tests.TestContexts;
    using Flower.Tests.TestDoubles;
    using Flower.Works;
    using Xunit;

    public class ExecutableWorkTests
    {
        [Fact]
        public void ExecutedWorkReferencesCorrectWork()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);

            // Act
            context.Trigger(3);

            // Assert
            Assert.Equal(context.Work, context.Executed.Single().Work);
        }

        [Fact]
        public void ExecutedWorkHasCorrectInput()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);

            // Act
            context.Trigger(3, 5, 7);

            // Assert
            Assert.Equal(new[] {3, 5, 7}, context.Executed.Select(w => w.Input));
        }

        [Fact]
        public void ExecutedWorkHasCorrectOutput()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);

            // Act
            context.Trigger(3, 5, 7);

            // Assert
            Assert.Equal(new[] {3, 5, 7}, context.Executed.Select(w => w.Output));
        }

        [Fact]
        public void ExecutedWorkHasWorkRunnerSet()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);

            // Act
            context.Trigger(3);

            // Assert
            Assert.NotNull(context.Executed.Single().WorkRunner);
        }

        [Fact]
        public async Task ExecutedWorkCannotBeExecutedAgain()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);
            context.Trigger(3);

            // Act
            var ex = await Record.ExceptionAsync(() => context.Executed.Single().Execute());

            // Assert
            Assert.IsType<InvalidOperationException>(ex);
        }

        [Fact]
        public void WorkerErrorExceptionIsSetOnExecuted()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            var result = context.Errored.Single();
            Assert.Equal(TestWorkerIntToIntThrowOnEven.ErrorMessage, result.Error.Message);
        }

        [Fact]
        public void WorkerErrorStateIsSetOnExecuted()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            var result = context.Errored.Single();
            Assert.Equal(ExecutableWorkState.Error, result.State);
        }

        [Fact]
        public void WorkerErrorOutputIsNotSetOnExecuted()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            var result = context.Errored.Single();
            Assert.Equal(default(int), result.Output);
        }

        [Fact]
        public void ContinuingAfterWorkerErrorLeavesWorkActive()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.Active, context.Work.State);
        }

        [Fact]
        public void WorkerErrorIsIgnoredOnOutput()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.Active, context.Work.State);
            Assert.Equal(new[] {3, 5}, context.Output);
        }

        [Fact]
        public void WorkerErrorCanBeNotifiedOnExecutedBeforeCompletingWork()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.CompleteWork);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.WorkerError, context.Work.State);
            Assert.Equal(new[] {3}, context.Output);
            Assert.Equal(new[] {3}, context.Executed.Select(w => w.Output));
            Assert.Equal(new[] {0}, context.Errored.Select(w => w.Output));
            Assert.NotNull(context.Work.LastError);
            Assert.Equal(ExecutableWorkState.Error, context.Errored.Single().State);
            Assert.Equal(TestWorkerIntToIntThrowOnEven.ErrorMessage, context.Errored.Single().Error.Message);
        }

        [Fact]
        public void WorkerErrorCanBeSwallowedOnExecuted()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.Active, context.Work.State);
            Assert.Equal(new[] {3, 5}, context.Executed.Where(executed => executed.Error == null).Select(w => w.Output));
        }

        [Fact]
        public void WorkerErrorCanBeSwallowedOnOutput()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.Continue);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.Active, context.Work.State);
            Assert.Equal(new[] {3, 5}, context.Output);
        }

        [Fact]
        public void WorkerErrorCanCompleteWork()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.CompleteWork);

            // Act
            context.Trigger(3, 4, 5);

            // Assert
            Assert.Equal(WorkState.WorkerError, context.Work.State);
            Assert.Equal(new[] {3}, context.Executed.Select(w => w.Output));
        }

        [Fact]
        public void WorkerErrorCanCompleteWorkAndThrow()
        {
            // Arrange
            var context = new IntToIntWorkerThrowOnEvenContext(WorkerErrorMode.CompleteWorkAndThrow);

            // Act
            var ex = Record.Exception(() => context.Trigger(3, 4, 5));

            // Assert
            Assert.IsType<ArgumentException>(ex);
            Assert.Equal(WorkState.WorkerError, context.Work.State);
            Assert.Equal(new[] {3}, context.Executed.Select(w => w.Output));
        }

        [Fact]
        public void WorkerErrorShownAsSuchWhenWorkCompletes()
        {
            // Arrange
            var options = new WorkOptions(WorkRegisterMode.Activated, workerErrorMode: WorkerErrorMode.CompleteWork);
            var workRegistry = new WorkRegistry(options);
            var trigger = new Subject<int>();
            var work = workRegistry.RegisterWorker(trigger, new TestWorkerIntToIntThrowOnEven());
            WorkState? result = null;
            work.Executed.Subscribe(_ => { }, () => result = work.State);

            // Act
            trigger.OnNext(3);
            trigger.OnNext(4);
            trigger.OnNext(5);

            // Assert
            Assert.Equal(WorkState.WorkerError, result);
        }
    }
}
