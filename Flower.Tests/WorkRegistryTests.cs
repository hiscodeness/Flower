using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xunit;

namespace Flower.Tests
{
    public class WorkRegistryTests
    {
        [Fact]
        public void RegisteringAndPipingWorkersCreatesAWorkflow()
        {
            // Arrange
            IList<int> expected = Enumerable.Range(0, 3).ToList();
            IList<int> result = new List<int>();
            var workPlanner = new WorkRegistry();
            var trigger = expected.ToObservable().Publish();

            // Act
            var output = workPlanner.Register(trigger, new TestWorkerInt2String())
                                    .Pipe(new TestWorkerString2Int())
                                    .Output
                                    .ToList();
            output.Subscribe(r => result = r);
            trigger.Connect();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void WorkerIsExecutedWhenRegisteredWorkIsExecuted()
        {
            // Arrange
            var subject = new Subject<int>();
            var workPlanner = new WorkRegistry();
            var plannedWork = workPlanner.Register(subject, new TestWorkerIntSquared());
            var result = 0;
            plannedWork.Output.SingleOrDefaultAsync().Subscribe(i => result = i);

            // Act
            subject.OnNext(42);
            subject.OnCompleted();

            // Assert
            Assert.Equal(TestWorkerIntSquared.WorkerFunc(42), result);
        }
    }
}