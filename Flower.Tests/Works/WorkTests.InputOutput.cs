using System;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Flower.Workers;
using Flower.Works;
using Xunit;

namespace Flower.Tests.Works
{
    using System.Collections.Generic;

    public class InputOutputWorkTests
    {
        [Fact]
        public void CompletedWorkCannotBeActivated()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry();
            var work = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());
            workRegistry.Complete(work);

            // Act
            var ex = Record.Exception(() => work.Activate());

            // Assert
            Assert.IsType<InvalidOperationException>(ex);
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
            var createWorkerScope = WorkerScope.FromInstance(new TestWorkerIntToIntSquared());

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
            var registry = new WorkRegistry();
            var work = registry.RegisterWorker(trigger, new TestWorkerIntToIntSquared());
            ITriggeredFuncWork<int, int> triggeredWork = null;

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
            var work = registry.RegisterWorker(trigger, new TestWorkerIntToIntThrowOnEven());
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
            var work = registry.RegisterWorker(trigger, new TestWorkerIntToIntThrowOnEven());
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