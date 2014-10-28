using System;
using System.Linq;
using FakeItEasy;
using Flower.WorkRunners;
using Flower.Works;
using Xunit;

namespace Flower.Tests.WorkRunners
{
    public class ImmediateWorkRunnerTests
    {
        [Fact]
        public void ThereAreNoTasksAfterCreation()
        {
            // Act
            var workRunner = new ImmediateWorkRunner();

            // Assert
            Assert.Empty(workRunner.PendingWorks);
            Assert.Empty(workRunner.RunningWorks);
        }

        [Fact]
        public void RunningTaskExistsDuringWorkerRun()
        {
            // Arrange
            var workRunner = new ImmediateWorkRunner();
            var result = workRunner.RunningWorks;
            var work = A.Fake<ITriggeredWork>();
            A.CallTo(() => work.Execute()).Invokes(_ => result = workRunner.RunningWorks.ToList());

            // Act
            workRunner.Submit(work);

            // Assert
            Assert.Equal(work, result.Single());
        }

        [Fact]
        public void DisposingDisposesRunningWorks()
        {
            // Arrange
            var workRunner = new ImmediateWorkRunner();
            var work = A.Fake<ITriggeredWork>();
            A.CallTo(() => work.Execute()).Invokes(_ => workRunner.Dispose());

            // Act, Assert
            Assert.Throws<ObjectDisposedException>(() => workRunner.Submit(work));
        }
    }
}
