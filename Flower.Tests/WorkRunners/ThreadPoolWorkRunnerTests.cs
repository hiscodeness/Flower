namespace Flower.Tests.WorkRunners
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Flower.WorkRunners;
    using Flower.Works;
    using Xunit;

    public class ThreadPoolWorkRunnerTests 
    {
        [Theory]
        [InlineData(200, 100)]
        [InlineData(1000, 5)]
        public void ThreadPoolRunnerExecutesAllWorksInParallel(int delayInMilliseconds, int workCount)
        {
            // Arrange
            var countdown = new CountdownEvent(workCount);
            var workRunner = new ThreadPoolWorkRunner();
            var workRunnerSnapshots = new ConcurrentBag<WorkRunnerSnapshot>();

            // Act
            for (var i = 0; i < workCount; i++)
            {
                workRunner.Submit(new MockExecutableWork(countdown, delayInMilliseconds, workRunnerSnapshots, workRunner));
            }

            // Assert
            countdown.Wait(TimeSpan.FromSeconds(20));
            Thread.Sleep(100); // Allow background thread to remove the work from workRunner
            Assert.Equal(workCount, workRunnerSnapshots.Count);
            Assert.True(workRunnerSnapshots.Select(state => state.PendingWorks.Count).All(count => count == 0));
            foreach (var executingWorkCount in workRunnerSnapshots.Select(state => state.ExecutingWorks.Count))
            {
                Assert.InRange(executingWorkCount, 1, workCount);
            }
            Assert.Equal(0, workRunner.ExecutingWorks.Count());
        }

        private class MockExecutableWork : IExecutableWork
        {
            private readonly CountdownEvent countdown;
            private readonly int delayInMilliseconds;
            private readonly ConcurrentBag<WorkRunnerSnapshot> workRunnerSnapshots;

            public MockExecutableWork(CountdownEvent countdown, int delayInMilliseconds, ConcurrentBag<WorkRunnerSnapshot> workRunnerSnapshots, IWorkRunner workRunner)
            {
                this.countdown = countdown;
                this.delayInMilliseconds = delayInMilliseconds;
                this.workRunnerSnapshots = workRunnerSnapshots;
                WorkRunner = workRunner;
            }

            public IWork Work { get; }
            public IWorkRunner WorkRunner { get; }
            public ExecutableWorkState State { get; }
            public void Execute()
            {
                Thread.Sleep(delayInMilliseconds);
                workRunnerSnapshots.Add(GetSnapshot(WorkRunner));
                countdown.Signal();
            }

            public Exception Error { get; }
            public IScope<object> WorkerScope { get; }
        }

        private static WorkRunnerSnapshot GetSnapshot(IWorkRunner workRunner)
        {
            return
                new WorkRunnerSnapshot
                {
                    PendingWorks = workRunner.PendingWorks.ToList(),
                    ExecutingWorks = workRunner.ExecutingWorks.ToList()
                };
        }

        private struct WorkRunnerSnapshot
        {
            public List<IExecutableWork> PendingWorks { get; set; }
            public List<IExecutableWork> ExecutingWorks { get; set; }
        }
    }
}
