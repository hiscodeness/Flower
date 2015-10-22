using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Flower.Works;
using Xunit;

namespace Flower.Tests.Works
{
    public class TriggeredWorkTests
    {
        [Fact]
        public void TriggeredWorkReferencesCorrectWork()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var work = workRegistry.RegisterWorker(trigger, new TestWorkerIntToIntSquared());
            ITriggeredFuncWork<int, int> triggeredWork = null;
            work.Triggered.Subscribe(w => triggeredWork = w);

            // Act
            trigger.OnNext(2);

            // Assert
            Assert.Equal(work, triggeredWork.Work);
        }

        [Fact]
        public void TriggeredWorkHasCorrectInput()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var work = workRegistry.RegisterWorker(trigger, new TestWorkerIntToIntSquared());
            ITriggeredFuncWork<int, int> triggeredWork = null;
            work.Triggered.Subscribe(w => triggeredWork = w);

            // Act
            trigger.OnNext(2);

            // Assert
            Assert.Equal(2, triggeredWork.Input);
        }

        [Fact]
        public void TriggeredWorkHasWorkRunnerSet()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var work = workRegistry.RegisterWorker(trigger, new TestWorkerIntToIntSquared());
            ITriggeredFuncWork<int, int> triggeredWork = null;
            work.Triggered.Subscribe(w => triggeredWork = w);

            // Act
            trigger.OnNext(2);

            // Assert
            Assert.NotNull(triggeredWork.WorkRunner);
        }

        [Fact]
        public void WhenExecutedHasCorrectOutputValue()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var work = workRegistry.RegisterWorker(trigger, new TestWorkerIntToIntSquared());
            var outputs = new List<int>();
            work.Executed.Subscribe(w => outputs.Add(w.Output));

            // Act
            trigger.OnNext(2);

            // Assert
            Assert.Equal(TestWorkerIntToIntSquared.WorkerFunc(2), outputs.Single());
        }
    }
}
