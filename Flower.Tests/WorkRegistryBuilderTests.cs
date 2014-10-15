using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Flower.Works;
using Xunit;

namespace Flower.Tests
{
    public class WorkRegistryBuilderTests
    {
        [Fact]
        public void RegisteringAndPipingWorkersCreatesAWorkflow()
        {
            // Arrange
            IList<int> result = new List<int>();
            var workRegistryBuilder = new WorkRegistryBuilder();
            var expected = Enumerable.Range(0, 3).ToList();
            var output =
                workRegistryBuilder.Register(expected.ToObservable(), new TestWorkerInt2String())
                                   .Pipe(new TestWorkerString2Int())
                                   .Output.ToList();

            output.Subscribe(r => result = r);

            // Act
            workRegistryBuilder.Build();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void WorkRegistryBuilderActivatesAllWorksWhenBuildingWorkRegistry()
        {
            // Arrange
            var subject = new Subject<int>();
            var workflowBuilder = new WorkRegistryBuilder();
            workflowBuilder.Register(subject, TestWorkers.Int2StringWorker);
            workflowBuilder.Register(subject, TestWorkers.IntSquaredWorker);
            workflowBuilder.Register(subject, TestWorkers.Int2StringWorker);

            // Act
            var workflow = workflowBuilder.Build();

            // Assert
            Assert.True(workflow.Works.All(IsActive));
        }

        [Fact]
        public void WorkRegistryBuilderBuildsRegistryWithCorrectCountOfWorks()
        {
            // Arrange
            var subject = new Subject<int>();
            var workflowBuilder = new WorkRegistryBuilder();
            workflowBuilder.Register(subject, TestWorkers.Int2StringWorker);
            workflowBuilder.Register(subject, TestWorkers.IntSquaredWorker);
            workflowBuilder.Register(subject, TestWorkers.Int2StringWorker);

            // Act
            var workflow = workflowBuilder.Build();

            // Assert
            Assert.Equal(3, workflow.Works.Count());
        }

        private static bool IsActive(IWork work)
        {
            return work.State == WorkState.Active;
        }
    }
}