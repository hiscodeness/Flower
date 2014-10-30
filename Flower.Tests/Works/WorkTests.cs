using System;
using FakeItEasy;
using Xunit;

namespace Flower.Tests.Works
{
    public partial class WorkTests
    {
        [Fact]
        public void CanRegisterWorkerWithoutInput()
        {
            // Arrange
            var workRegistry = new WorkRegistry();
            var trigger = A.Fake<IObservable<int>>();
            var worker = A.Fake<IWorker>();
            
            // Act
            var work = workRegistry.Register(trigger, worker);

            // Assert
            Assert.Equal(worker, work.Registration.WorkerResolver.Resolve());
        }
    }
}
