using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FakeItEasy;
using Flower.Tests.TestDoubles;
using Flower.WorkRunners;
using Flower.Works;
using Xunit;

namespace Flower.Tests
{
    using System.Threading.Tasks;

    public class WorkRegistryTests
    {
        [Fact]
        public void UsingCustomWorkRunnersIsEasy()
        {
            // Arrange
            var workRunner = new BackgroundThreadQueueWorkRunner();

            // Assert
            var workRegistry = new WorkRegistry(new WorkOptions(workRunner));
        }

        [Fact]
        public void RegisteringAndPipingWorkersCreatesWorkflows()
        {
            // Arrange
            IList<int> expected = Enumerable.Range(0, 3).ToList();
            IList<int> result = new List<int>();
            var workRegistry = new WorkRegistry(new WorkOptions(WorkRegisterMode.Suspended));

            // Act
            var output = workRegistry.RegisterWorker(expected.ToObservable(), new TestWorkerIntToString())
                                    .Pipe(new TestWorkerStringToInt())
                                    .Output
                                    .ToList();
            output.Subscribe(r => result = r);
            workRegistry.Activate();

            // Assert
            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void ManuallyCompletingWorkCompletesPipedWorks()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var work1 = workRegistry.RegisterWorker(trigger, new TestWorkerIntToString());
            var work2 = work1.Pipe(new TestWorkerStringToInt());
            var work3 = work2.Pipe(new TestWorkerIntToIntSquared());

            // Act
            work1.Complete();

            // Assert
            Assert.Equal(WorkState.Completed, work1.State);
            Assert.Equal(WorkState.Completed, work2.State);
            Assert.Equal(WorkState.Completed, work3.State);
        }

        [Fact]
        public void WorkerIsExecutedWhenRegisteredWorkIsExecuted()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry();
            var work = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());
            var result = 0;
            work.Output.SingleOrDefaultAsync().Subscribe(i => result = i);

            // Act
            subject.OnNext(42);
            subject.OnCompleted();

            // Assert
            Assert.Equal(TestWorkerIntToIntSquared.WorkerFunc(42), result);
        }

        [Fact]
        public void CompletingWorkCompletesTriggeredObservable()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var work = workRegistry.RegisterWorker(trigger, new TestWorkerIntToString());
            var workTriggeredCompleted = false;
            work.Triggered.Subscribe(_ => { }, () => workTriggeredCompleted = true);

            // Act
            work.Complete();

            // Assert
            Assert.True(workTriggeredCompleted);
        }

        [Fact]
        public void CompletingWorkCompletesExecutedObservable()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var work = workRegistry.RegisterWorker(trigger, new TestWorkerIntToString());
            var workExecutedCompleted = false;
            work.Executed.Subscribe(_ => { }, () => workExecutedCompleted = true);

            // Act
            work.Complete();

            // Assert
            Assert.True(workExecutedCompleted);
        }

        [Fact]
        public void WorkCanBeCompletedManually()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry();
            var work = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());

            // Act
            workRegistry.Complete(work);

            // Assert
            Assert.False(subject.HasObservers);
            Assert.Equal(WorkState.Completed, work.State);
            Assert.False(workRegistry.Works.Any());
        }

        [Fact]
        public void WorksCanBeCompletedManually()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry();
            var work1 = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());
            var work2 = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());
            var work3 = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());

            // Act
            workRegistry.Complete(work1);
            workRegistry.Complete(work3);

            // Assert
            Assert.True(subject.HasObservers);
            Assert.Equal(WorkState.Completed, work1.State);
            Assert.Equal(WorkState.Active, work2.State);
            Assert.Equal(WorkState.Completed, work3.State);
            Assert.Equal(work2, workRegistry.Works.Single());
        }


        [Fact]
        public void WorkCanBeCompletedManuallyExactlyOnce()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry();
            var work = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());
            workRegistry.Complete(work);

            // Act
            var ex = Record.Exception(() => workRegistry.Complete(work));

            // Assert
            Assert.IsType<InvalidOperationException>(ex);
        }

        [Fact]
        public void AllWorksCanBeSuspended()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry();
            workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared())
                        .Pipe(new TestWorkerIntToString())
                        .Pipe(new TestWorkerStringToInt());

            // Act 
            workRegistry.Suspend();

            // Assert
            Assert.Equal(3, workRegistry.Works.Count());
            Assert.True(workRegistry.Works.All(work => work.State == WorkState.Suspended));
        }
        
        [Fact]
        public void AllWorksCanBeActivated()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry(new WorkOptions(WorkRegisterMode.Suspended));
            workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared())
                        .Pipe(new TestWorkerIntToString())
                        .Pipe(new TestWorkerStringToInt());

            // Act 
            workRegistry.Activate();

            // Assert
            Assert.Equal(3, workRegistry.Works.Count());
            Assert.True(workRegistry.Works.All(work => work.State == WorkState.Active));
        }

        [Fact]
        public void ActivatingAllActivatesOnlySuspendedWorks()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry(new WorkOptions(WorkRegisterMode.Suspended));
            var work1 = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());
            var work2 = work1.Pipe(new TestWorkerIntToString());
            var work3 = work2.Pipe(new TestWorkerStringToInt());
            work3.Activate();

            // Act 
            workRegistry.Activate();

            // Assert
            Assert.Equal(3, workRegistry.Works.Count());
            Assert.True(workRegistry.Works.All(work => work.State == WorkState.Active));
        }

        [Fact]
        public void ActivatingSingleWorkActivatesOnlyThatWork()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry(new WorkOptions(WorkRegisterMode.Suspended));
            var work1 = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());
            var work2 = work1.Pipe(new TestWorkerIntToString());
            var work3 = work2.Pipe(new TestWorkerStringToInt());

            // Act 
            work1.Activate();
            work3.Activate();

            // Assert
            Assert.Equal(3, workRegistry.Works.Count());
            Assert.Equal(WorkState.Active, work1.State);
            Assert.Equal(WorkState.Suspended, work2.State);
            Assert.Equal(WorkState.Active, work3.State);
        }

        [Fact]
        public void ManuallyCompletingWorkAlsoCompletesDependentPipedWorks()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry();
            var work1 = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());
            var work2 = work1.Pipe(new TestWorkerIntToString());
            var work3 = work2.Pipe(new TestWorkerStringToInt());
            var work4 = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());

            // Act 
            work2.Complete();

            // Assert
            Assert.Equal(new[] { work1, work4 }, workRegistry.Works);
            Assert.Equal(WorkState.Active, work1.State);
            Assert.Equal(WorkState.Completed, work2.State);
            Assert.Equal(WorkState.Completed, work3.State);
            Assert.Equal(WorkState.Active, work4.State);
        }
        
        [Fact]
        public void CompletingAllWorksIsPossibleAfterManuallyCompletingAWork()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry();
            var work1 = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());
            var work2 = work1.Pipe(new TestWorkerIntToString());
            var work3 = work2.Pipe(new TestWorkerStringToInt());
            var work4 = workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());
            work2.Complete();

            // Act 
            workRegistry.CompleteAll();

            // Assert
            Assert.Empty(workRegistry.Works);
            Assert.Equal(WorkState.Completed, work1.State);
            Assert.Equal(WorkState.Completed, work2.State);
            Assert.Equal(WorkState.Completed, work3.State);
            Assert.Equal(WorkState.Completed, work4.State);
        }

        [Fact]
        public void ItIsPossibleToCompleteAllWorks()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry();
            workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared())
                        .Pipe(new TestWorkerIntToString())
                        .Pipe(new TestWorkerStringToInt());
            workRegistry.RegisterWorker(subject, new TestWorkerIntToIntSquared());
            var works = workRegistry.Works.ToList();

            // Act 
            workRegistry.CompleteAll();

            // Assert
            Assert.Equal(0, workRegistry.Works.Count());
            Assert.True(works.All(work => work.State == WorkState.Completed));
        }

        [Fact]
        public void CannotCompleteNullWork()
        {
            // Arrange
            var workRegistry = new WorkRegistry();

            // Act
            var ex = Record.Exception(() => workRegistry.Complete(null));

            // Assert
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void CannotCompleteWorkIfNotRegisteredWithTheWorkRegistry()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var work = A.Fake<IWork>();

            // Act
            var ex = Record.Exception(() => workRegistry.Complete(work));

            // Assert
            Assert.IsType<InvalidOperationException>(ex);
        }

        [Fact]
        public void EachRegisteredWorkHasUniqueOptions()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            
            // Act
            var work1 = workRegistry.RegisterWorker(trigger, new TestWorkerInt());
            var work2 = workRegistry.RegisterWorker(
                trigger, new TestWorkerInt(), new WorkOptions(WorkRegisterMode.Suspended));

            // Assert
            Assert.NotEqual(workRegistry.Options, work1.Registration.Options);
            Assert.NotEqual(workRegistry.Options, work2.Registration.Options);
            Assert.NotEqual(work1.Registration.Options, work2.Registration.Options);
        }

        [Fact]
        public void ActionWithNoInputCanBeRegistered()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();

            // Act
            var ex = Record.Exception(() => workRegistry.RegisterMethod(trigger, () => Task.CompletedTask ));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void ActionWithInputCanBeRegistered()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();

            // Act
            var ex = Record.Exception(() => workRegistry.RegisterMethod(trigger, i => Task.CompletedTask));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void FuncCanBeRegistered()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();

            // Act
            var ex = Record.Exception(() => workRegistry.RegisterMethod(trigger, i => Task.FromResult(i.ToString(CultureInfo.InvariantCulture))));

            // Assert
            Assert.Null(ex);
        }
    }
}