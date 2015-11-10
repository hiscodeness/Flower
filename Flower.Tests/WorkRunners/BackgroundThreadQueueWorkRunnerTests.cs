using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Flower.WorkRunners;
using Flower.Works;
using Xunit;

namespace Flower.Tests.WorkRunners
{
    public class BackgroundThreadQueueWorkRunnerTests
    {
        [Theory]
        [InlineData(5, 100)]
        [InlineData(100, 5)]
        public async Task BackgroundThreadQueueExecutesOneWorkWhileOthersArePending(int delayInMilliseconds, int workCount)
        {
            // Arrange
            var countdown = new CountdownEvent(workCount);
            var workRunner = new BackgroundThreadQueueWorkRunner();
            var workRunnerSnapshots = new List<WorkRunnerSnapshot>();
            var executableWork = A.Fake<IExecutableWork>();
            A.CallTo(() => executableWork.Execute()).Invokes(_ =>
                {
                    Task.Delay(delayInMilliseconds).Wait();
                    workRunnerSnapshots.Add(GetSnapshot(workRunner));
                    countdown.Signal();
                });

            // Act
            for (var i = 0; i < workCount; i++)
            {
                await workRunner.Submit(executableWork);
            }

            // Assert
            countdown.Wait(TimeSpan.FromSeconds(10));
            Assert.Equal(workCount, workRunnerSnapshots.Count);
            Assert.True(workRunnerSnapshots.Select(state => state.ExecutingWorks.Count).All(count => count == 1));
            var maxPendingCount = Enumerable.Range(1, workCount-1).Aggregate((acc, x) => acc + x);
            Assert.InRange(workRunnerSnapshots.Sum(state => state.PendingWorks.Count), workCount, maxPendingCount);
        }

        [Fact]
        public async Task BackgroundThreadQueueDoesntWaitForAllTriggeredWorksToExecute()
        {
            // Arrange
            var manualResetEvent = new ManualResetEventSlim();
            var workRunner = new BackgroundThreadQueueWorkRunner();
            var executableWork = A.Fake<IExecutableWork>();
            var executedWorkCount = 0;
            A.CallTo(() => executableWork.Execute()).Invokes(
                _ =>
                    {
                        executedWorkCount++;
                        manualResetEvent.Set();
                        Task.Delay(100).Wait();
                    });
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Act
            for (var i = 0; i < 5; i++)
            {
                await workRunner.Submit(executableWork);
            }
            manualResetEvent.Wait(TimeSpan.FromSeconds(10));
            manualResetEvent.Reset();
            manualResetEvent.Wait(TimeSpan.FromSeconds(10));
            workRunner.Dispose();

            // Assert
            Assert.InRange(stopwatch.ElapsedMilliseconds, 0, 299);
            Assert.Equal(2, executedWorkCount);
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
