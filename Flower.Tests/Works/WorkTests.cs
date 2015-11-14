using System;
using System.Reactive.Subjects;
using FakeItEasy;
using Flower.Tests.TestDoubles;
using Flower.Works;
using Xunit;

namespace Flower.Tests.Works
{
    using System.Linq;
    using System.Threading;
    using Flower.Workers;
    using Flower.WorkRunners;

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
        public void CanGetErrorIfWorkerErrorsAndWorkCompletes()
        {
            // Arrange
            var countdown = new CountdownEvent(1);
            var trigger = new Subject<int>();
            var registry = new WorkRegistry(RegisterOptions.Default.With(new ThreadPoolWorkRunner()));
            registry.RegisterWorker(trigger, new TestWorkerIntToIntThrowOnEven());
            WorkerError workerError = null;
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

    }
}
