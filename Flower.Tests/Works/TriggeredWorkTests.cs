using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Xunit;

namespace Flower.Tests.Works
{
    public class TriggeredWorkTests
    {
        [Fact]
        public void WhenTriggeredHasCorrectOutputValue()
        {
            // Arrange
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var trigger = new Subject<int>();
            var work = workRegistry.Register(trigger, new TestWorkerIntToIntSquared());
            var outputs = new List<int>();
            work.Triggered.Subscribe(w => outputs.Add(w.Output));
            work.Executed.Subscribe(w => outputs.Add(w.Output));

            // Act
            trigger.OnNext(2);

            // Assert
            Assert.Equal(default(int), outputs.First());
        }

        [Fact]
        public void WhenExecutedHasCorrectOutputValue()
        {
            // Arrange
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var trigger = new Subject<int>();
            var work = workRegistry.Register(trigger, new TestWorkerIntToIntSquared());
            var outputs = new List<int>();
            work.Triggered.Subscribe(w => outputs.Add(w.Output));
            work.Executed.Subscribe(w => outputs.Add(w.Output));

            // Act
            trigger.OnNext(2);

            // Assert
            Assert.Equal(TestWorkerIntToIntSquared.WorkerFunc(2), outputs.Last());
        }
    }
}
