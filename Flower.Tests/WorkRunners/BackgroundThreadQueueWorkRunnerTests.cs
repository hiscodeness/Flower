using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flower.WorkRunners;
using Flower.Works;
using Xunit;

namespace Flower.Tests.WorkRunners
{
    public class BackgroundThreadQueueWorkRunnerTests
    {
        [Theory]
        [InlineData(20, 100)]
        [InlineData(200, 10)]
        public async Task BackgroundThreadQueueExecutesOneWorkWhileOthersArePending(int delayInMilliseconds, int workCount)
        {
            // Arrange
            var countdown = new CountdownEvent(workCount);
            var workRunner = new BackgroundThreadQueueWorkRunner();
            var workRunnerSnapshots = new List<WorkRunnerSnapshot>();
            var threadIds = new List<int>();

            // Act
            for (var i = 0; i < workCount; i++)
            {
                await workRunner.Submit(new SnapshottingExecutableWork(countdown, delayInMilliseconds, workRunnerSnapshots, threadIds, workRunner));
            }

            // Assert
            countdown.Wait(TimeSpan.FromSeconds(10));
            Assert.Equal(workCount, workRunnerSnapshots.Count);
            Assert.All(workRunnerSnapshots, snapshot => Assert.Equal(1, snapshot.ExecutingWorks.Count));
            Assert.All(threadIds, threadId => Assert.Equal(workRunner.ThreadId, threadId));
            var maxPendingCount = Enumerable.Range(1, workCount-1).Aggregate((acc, x) => acc + x);
            Assert.InRange(workRunnerSnapshots.Sum(state => state.PendingWorks.Count), workCount, maxPendingCount);
        }

        [Fact]
        public async Task DisposedBackgroundThreadQueueDoesntWaitForAllTriggeredWorksToExecute()
        {
            // Arrange
            var manualResetEvent = new ManualResetEventSlim();
            var workRunner = new BackgroundThreadQueueWorkRunner();
            var executableWorks = new List<IExecutableWork>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Act
            for (var i = 0; i < 5; i++)
            {
                await workRunner.Submit(new CountingExecutableWork(executableWorks, manualResetEvent));
            }
            manualResetEvent.Wait(TimeSpan.FromSeconds(10));
            manualResetEvent.Reset();
            manualResetEvent.Wait(TimeSpan.FromSeconds(10));
            workRunner.Dispose();

            // Assert
            Assert.InRange(stopwatch.ElapsedMilliseconds, 0, 299);
            Assert.Equal(2, executableWorks.Count);
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

        private class SnapshottingExecutableWork : IExecutableWork
        {
            private readonly CountdownEvent countdown;
            private readonly int delayInMilliseconds;
            private readonly IList<WorkRunnerSnapshot> workRunnerSnapshots;
            private readonly IWorkRunner workRunner;
            private readonly IList<int> threadIds; 

            public SnapshottingExecutableWork(
                CountdownEvent countdown,
                int delayInMilliseconds,
                IList<WorkRunnerSnapshot> workRunnerSnapshots,
                IList<int> threadIds,
                IWorkRunner workRunner)
            {
                this.countdown = countdown;
                this.delayInMilliseconds = delayInMilliseconds;
                this.workRunnerSnapshots = workRunnerSnapshots;
                this.threadIds = threadIds;
                this.workRunner = workRunner;
            }
            public IWork Work { get; }
            public IWorkRunner WorkRunner { get; }
            public ExecutableWorkState State { get; }
            public async Task Execute()
            {
                await Task.Delay(delayInMilliseconds);
                workRunnerSnapshots.Add(GetSnapshot(workRunner));
                countdown.Signal();
                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
            }

            public Exception Error { get; }
            public IScope<object> WorkerScope { get; }
        }

        private class CountingExecutableWork : IExecutableWork
        {
            private readonly List<IExecutableWork> executableWorks;
            private readonly ManualResetEventSlim manualResetEvent;

            public CountingExecutableWork(List<IExecutableWork> executableWorks, ManualResetEventSlim manualResetEvent)
            {
                this.executableWorks = executableWorks;
                this.manualResetEvent = manualResetEvent;
            }

            public IWork Work { get; }
            public IWorkRunner WorkRunner { get; }
            public ExecutableWorkState State { get; }
            public async Task Execute()
            {
                executableWorks.Add(this);
                manualResetEvent.Set();
                await Task.Delay(100);
            }

            public Exception Error { get; }
            public IScope<object> WorkerScope { get; }
        }
    }
}
