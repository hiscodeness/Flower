using System;
using System.Reactive.Subjects;
using Flower.Tests.TestDoubles;
using Xunit;

namespace Flower.Tests.Works
{
    public class WorkTests
    {
        [Fact]
        public void UnregisteredWorkCannotBeActivated()
        {
            // Arrange
            var subject = new Subject<int>();
            var workRegistry = new WorkRegistry(true);
            var work = workRegistry.Register(subject, TestWorkers.IntSquaredWorker);
            workRegistry.Unregister(work);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => work.Activate());
        }
    }
}