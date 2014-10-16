using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
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
            var workRegistryBuilder = WorkRegistryBuilder.Create();
            var expected = Enumerable.Range(0, 3).ToList();
            var output =
                workRegistryBuilder.Register(expected.ToObservable(), TestWorkers.Int2StringWorker)
                                   .Pipe(TestWorkers.String2IntWorker)
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
            var workRegistryBuilder = WorkRegistryBuilder.Create();
            workRegistryBuilder.Register(subject, TestWorkers.Int2StringWorker);
            workRegistryBuilder.Register(subject, TestWorkers.IntSquaredWorker);
            workRegistryBuilder.Register(subject, TestWorkers.Int2StringWorker);

            // Act
            var workRegistry = workRegistryBuilder.Build();

            // Assert
            Assert.True(workRegistry.Works.All(IsActive));
        }

        [Fact]
        public void WorkRegistryBuilderBuildsRegistryWithCorrectCountOfWorks()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistryBuilder = WorkRegistryBuilder.Create();
            workRegistryBuilder.Register(subject, TestWorkers.Int2StringWorker);
            workRegistryBuilder.Register(subject, TestWorkers.IntSquaredWorker);
            workRegistryBuilder.Register(subject, TestWorkers.Int2StringWorker);

            // Act
            var workRegistry = workRegistryBuilder.Build();

            // Assert
            Assert.Equal(3, workRegistry.Works.Count());
        }

        private static bool IsActive(IWork work)
        {
            return work.State == WorkState.Active;
        }
    }
}