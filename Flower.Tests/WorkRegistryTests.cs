using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FakeItEasy;
using Flower.Tests.TestDoubles;
using Flower.Works;
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
            var workRegistry = new WorkRegistry();

            // Act
            var output = workRegistry.Register(expected.ToObservable(), new TestWorkerIntToString())
                                    .Pipe(new TestWorkerStringToInt())
                                    .Output
                                    .ToList();
            output.Subscribe(r => result = r);
            workRegistry.ActivateAllWorks();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void WorkerIsExecutedWhenRegisteredWorkIsExecuted()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var work = workRegistry.Register(subject, new TestWorkerIntToIntSquared());
            var result = 0;
            work.Output.SingleOrDefaultAsync().Subscribe(i => result = i);

            // Act
            subject.OnNext(42);
            subject.OnCompleted();

            // Assert
            Assert.Equal(TestWorkerIntToIntSquared.WorkerFunc(42), result);
        }

        [Fact]
        public void WorkCanBeUnregistered()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var work = workRegistry.Register(subject, new TestWorkerIntToIntSquared());

            // Act
            workRegistry.Unregister(work);

            // Assert
            Assert.False(subject.HasObservers);
            Assert.Equal(WorkState.Unregistered, work.State);
            Assert.False(workRegistry.Works.Any());
        }

        [Fact]
        public void WorkCanBeUnregisteredOnce()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            var work = workRegistry.Register(subject, new TestWorkerIntToIntSquared());
            workRegistry.Unregister(work);

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() => workRegistry.Unregister(work));
        }

        [Fact]
        public void AllWorksCanBeSuspended()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            workRegistry.Register(subject, new TestWorkerIntToIntSquared())
                        .Pipe(new TestWorkerIntToString())
                        .Pipe(new TestWorkerStringToInt());

            // Act 
            workRegistry.SuspendAllWorks();

            // Assert
            Assert.Equal(3, workRegistry.Works.Count());
            Assert.True(workRegistry.Works.All(work => work.State == WorkState.Suspended));
        }
        
        [Fact]
        public void AllWorksCanBeActivated()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry();
            workRegistry.Register(subject, new TestWorkerIntToIntSquared())
                        .Pipe(new TestWorkerIntToString())
                        .Pipe(new TestWorkerStringToInt());

            // Act 
            workRegistry.ActivateAllWorks();

            // Assert
            Assert.Equal(3, workRegistry.Works.Count());
            Assert.True(workRegistry.Works.All(work => work.State == WorkState.Active));
        }

        [Fact]
        public void DisposeUnregistersAllWorks()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = WorkRegistryFactory.CreateAutoActivating();
            workRegistry.Register(subject, new TestWorkerIntToIntSquared())
                        .Pipe(new TestWorkerIntToString())
                        .Pipe(new TestWorkerStringToInt());
            var works = workRegistry.Works.ToList();

            // Act 
            workRegistry.Dispose();

            // Assert
            Assert.Equal(0, workRegistry.Works.Count());
            Assert.True(works.All(work => work.State == WorkState.Unregistered));
        }

        [Fact]
        public void CannotUnregisterNull()
        {
            // Arrange
            var workRegistry = new WorkRegistry();

            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => workRegistry.Unregister(null));
        }
        
        [Fact]
        public void CannotUnregisterIfNotRegistered()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var work = A.Fake<IWorkBase>();

            // Act / Assert
            Assert.Throws<InvalidOperationException>(() => workRegistry.Unregister(work));
        }
    }
}