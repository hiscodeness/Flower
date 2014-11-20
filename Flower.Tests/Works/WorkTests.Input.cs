using System;
using System.Linq;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Flower.Works;
using Xunit;

namespace Flower.Tests.Works
{
    public partial class WorkTests
    {
        [Fact]
        public void CanRegisterWorkAcceptingInput()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var worker = new TestWorkerInt();
            workRegistry.Register(trigger, worker);

            // Act
            trigger.OnNext(42);

            // Assert
            Assert.Equal(42, worker.Inputs.Single());
        }

        [Fact]
        public void WorkWithInputTriggered()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = new WorkRegistry();
            var work = registry.Register(trigger, new TestWorkerInt());
            ITriggeredActionWork<int> triggeredWork = null;

            // Act
            work.Triggered.Subscribe(tw => triggeredWork = tw);
            trigger.OnNext(42);

            // Assert
            Assert.NotNull(triggeredWork);
        }
    }
}