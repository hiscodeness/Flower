using System;
using System.Linq;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Flower.Works;
using Xunit;

namespace Flower.Tests.Works
{
    using System.Collections.Generic;

    public class InputWorkTests
    {
        [Fact]
        public void CanRegisterWorkAcceptingInput()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var worker = new TestWorkerInt();
            workRegistry.RegisterWorker(trigger, worker);

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
            var work = registry.RegisterWorker(trigger, new TestWorkerInt());
            ITriggeredActionWork<int> triggeredWork = null;

            // Act
            work.Triggered.Subscribe(tw => triggeredWork = tw);
            trigger.OnNext(42);

            // Assert
            Assert.NotNull(triggeredWork);
        }


        [Fact]
        public void ErroredDoNotShowUpInExecuted()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = new WorkRegistry();
            var work = registry.RegisterWorker(trigger, new TestWorkerIntThrowOnEven());
            var executedWorks = new List<IExecutableWork>();
            work.Executed.Subscribe(executedWork => executedWorks.Add(executedWork));

            // Act
            trigger.OnNext(3);
            trigger.OnNext(4);

            // Assert
            Assert.Equal(WorkState.Active, work.State);
            Assert.Equal(1, executedWorks.Count);
            Assert.All(executedWorks, executedWork => Assert.Null(executedWork.Error));
        }

        [Fact]
        public void ExecutedDoNotShowUpInErrored()
        {
            // Arrange
            var trigger = new Subject<int>();
            var registry = new WorkRegistry();
            var work = registry.RegisterWorker(trigger, new TestWorkerIntThrowOnEven());
            var erroredWorks = new List<IExecutableWork>();
            work.Errored.Subscribe(erroredWork => erroredWorks.Add(erroredWork));

            // Act
            trigger.OnNext(3);
            trigger.OnNext(4);

            // Assert
            Assert.Equal(WorkState.Active, work.State);
            Assert.Equal(1, erroredWorks.Count);
            Assert.All(erroredWorks, erroredWork => Assert.NotNull(erroredWork.Error));
        }
    }
}