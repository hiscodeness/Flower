namespace Flower.Tests.WorkRunners
{
    using System.Linq;
    using System.Threading.Tasks;
    using FakeItEasy;
    using Flower.WorkRunners;
    using Flower.Works;
    using Xunit;

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
        public async Task RunningTaskExistsDuringWorkerRun()
        {
            // Arrange
            var workRunner = new ImmediateWorkRunner();
            var result = workRunner.ExecutingWorks;
            var work = A.Fake<IExecutableActionWork>();
            A.CallTo(() => work.Execute())
                .Invokes(() => { result = workRunner.ExecutingWorks.ToList(); })
                .Returns(Task.CompletedTask);

            // Act
            await workRunner.Submit(work);

            // Assert
            Assert.Equal(work, result.Single());
        }
    }
}
