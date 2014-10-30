using System.Linq;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Flower.Workers;
using Xunit;

namespace Flower.Tests.Works
{
    public partial class WorkTests
    {
        [Fact]
        public void CanRegisterWorkAcceptingInput()
        {
            // Arrange
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var trigger = new Subject<int>();
            var worker = new TestWorkerInt();
            workRegistry.Register(trigger, worker);

            // Act
            trigger.OnNext(42);

            // Assert
            Assert.Equal(42, worker.Inputs.Single());
        }

        [Fact]
        public void PipingExecutedToWorkerSucceeds()
        {
            // Arrange
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var trigger = new Subject<int>();
            var work = workRegistry.Register(trigger, new TestWorkerInt());
            
            // Act
            var pipedWorker = new TestWorkerInt();
            work.Pipe(pipedWorker);
            trigger.OnNext(42);

            // Assert
            Assert.Equal(42, pipedWorker.Inputs.Single());
        }

        [Fact]
        public void PipingExecutedToWorkerResolverSucceeds()
        {
            // Arrange
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var trigger = new Subject<int>();
            var work = workRegistry.Register(trigger, new TestWorkerInt());

            // Act
            var pipedWorker = new TestWorkerInt();
            work.Pipe(WorkerResolver.CreateFromInstance(pipedWorker));
            trigger.OnNext(42);

            // Assert
            Assert.Equal(42, pipedWorker.Inputs.Single());
        }
    }
}
