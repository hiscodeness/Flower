using System;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Flower.Workers;
using Flower.Works;
using Xunit;

namespace Flower.Tests.Works
{
    public partial class WorkTests
    {
        [Fact]
        public void CompletedWorkCannotBeActivated()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var work = workRegistry.Register(subject, new TestWorkerIntToIntSquared());
            workRegistry.Complete(work);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => work.Activate());
        }

        [Fact]
        public void PipingWorkOutputToWorkerInputSucceeds()
        {
            // Arrange
            var work = new TestWorkIntToIntSquared();
            var worker = new TestWorkerIntToIntSquared();

            // Act
            var pipedWork = work.Pipe(worker);

            // Assert
            Assert.NotNull(pipedWork.Registration.Trigger);
            Assert.Equal(work.Output, pipedWork.Registration.Trigger);
        }

        [Fact]
        public void PipingWorkOutputToWorkerScopeSucceeds()
        {
            // Arrange
            var work = new TestWorkIntToIntSquared();
            var createWorkerScope = WorkerScope.Instance(new TestWorkerIntToIntSquared());

            // Act
            var pipedWork = work.Pipe(createWorkerScope);

            // Assert
            Assert.NotNull(pipedWork.Registration.Trigger);
            Assert.Equal(work.Output, pipedWork.Registration.Trigger);
        }

        [Fact]
        public void WorkWithInputAndOutputTriggered()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = WorkRegistryFactory.CreateAutoActivating();
            var work = registry.Register(trigger, new TestWorkerIntToIntSquared());
            ITriggeredFuncWork<int, int> triggeredWork = null;

            // Act
            work.Triggered.Subscribe(tw => triggeredWork = tw);
            trigger.OnNext(42);

            // Assert
            Assert.NotNull(triggeredWork);
        }
    }
}