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
            Assert.Empty(workRunner.ExecutingWorks);
        }

        [Fact]
        public void RunningTaskExistsDuringWorkerRun()
        {
            // Arrange
            var workRunner = new ImmediateWorkRunner();
            var result = workRunner.ExecutingWorks;
            var work = A.Fake<IExecutableActionWork>();
            A.CallTo(() => work.Execute()).Invokes(_ => result = workRunner.ExecutingWorks.ToList());

            // Act
            workRunner.Submit(work);

            // Assert
            Assert.Equal(work, result.Single());
        }
    }
}
