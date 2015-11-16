namespace Flower.Tests.Works
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using FakeItEasy;
    using Flower.Tests.TestDoubles;
    using Flower.Works;
    using Flower.Workers;
    using Flower.WorkRunners;
    using Xunit;

    public partial class WorkTests
    {
        [Fact]
        public void CanRegisterWorkerWithoutInput()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = A.Fake<IObservable<int>>();
            var worker = A.Fake<IWorker>();

            // Act
            var work = workRegistry.RegisterWorker(trigger, worker);

            // Assert
            Assert.Equal(worker, work.Registration.CreateWorkerScope().Worker);
        }

        [Fact]
        public void WorkTriggered()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = new WorkRegistry();
            var work = registry.RegisterWorker(trigger, new TestWorker());
            ITriggeredActionWork triggeredWork = null;

            // Act
            work.Triggered.Subscribe(tw => triggeredWork = tw);
            trigger.OnNext(42);

            // Assert
            Assert.NotNull(triggeredWork);
        }

        [Fact]
        public void CanGetLastWorkerError()
        {
            // Arrange
            var trigger = new Subject<int>();
            var work = new WorkRegistry().RegisterWorker(trigger, new TestWorkerIntToIntThrowOnEven());

            // Act
            trigger.OnNext(42);

            // Assert
            Assert.NotNull(work.LastError);
            Assert.Equal(TestWorkerIntToIntThrowOnEven.ErrorMessage, work.LastError.Error.Message);
        }

        [Fact]
        public void CanGetErrorIfWorkerErrorsAndWorkCompletes()
        {
            // Arrange
            var countdown = new CountdownEvent(1);
            var trigger = new Subject<int>();
            var registry = new WorkRegistry(WorkOptions.Default.With(WorkerErrorMode.CompleteWorkAndThrow).With(new ThreadPoolWorkRunner()));
            registry.RegisterWorker(trigger, new TestWorkerIntToIntThrowOnEven());
            WorkerErrorBase workerError = null;
            registry.Works.Single().Completed.Subscribe(
                completed =>
                {
                    workerError = completed.LastError;
                    countdown.Signal();
                });
            // Act
            trigger.OnNext(42);

            // Assert
            countdown.Wait();
            Assert.NotNull(workerError);
            Assert.NotNull(workerError.Error);
            Assert.NotNull(workerError.Worker);
        }


        [Fact]
        public void WorkerErrorCanBeIgnored()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = new WorkRegistry(WorkOptions.Default.With(WorkerErrorMode.Continue));
            var work1 = registry.RegisterWorker(trigger, new TestWorkerIntToIntThrowOnEven());
            var work2 = work1.Pipe(new TestWorkerIntToIntSquared());
            var work2Output = new List<int>();
            work2.Output.Subscribe(work2Output.Add);

            // Act
            trigger.OnNext(3);
            var ex = Record.Exception(() => trigger.OnNext(4));
            trigger.OnNext(5);

            // Assert
            Assert.Equal(WorkState.Active, work1.State);
            Assert.Equal(WorkState.Active, work2.State);
            Assert.Equal(new[] {3*3, 5*5}, work2Output);
        }
    }
}
