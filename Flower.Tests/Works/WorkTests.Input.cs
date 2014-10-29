using System.Linq;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
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
            var work = workRegistry.Register(trigger, worker);

            // Act
            trigger.OnNext(42);

            // Assert
            Assert.Equal(42, worker.Inputs.Single());
        }

        [Fact]
        public void PipingExecutedToWorkerInputSucceeds()
        {
            // Arrange
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var trigger = new Subject<int>();
            var pipedWorker = new TestWorkerInt();
            var work = workRegistry.Register(trigger, new TestWorkerInt());
            
            // Act
            work.Pipe(pipedWorker);
            trigger.OnNext(42);

            // Assert
            Assert.Equal(42, pipedWorker.Inputs.Single());
        }
    }
}
