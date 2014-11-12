using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Flower.Tests.TestDoubles;
using Flower.Works;
using Xunit;

namespace Flower.Tests.Works
{
    public class ExecutableWorkTests
    {
        [Fact]
        public void WorkerErrorIsSetOnExecutableWork()
        {
            // Arrange
            var options = new WorkRegistryOptions(
                RegisterWorkBehavior.RegisterActivated, workerErrorBehavior: WorkerErrorBehavior.RaiseExecutedAndContinue);
            var workRegistry = new WorkRegistry(options);
            var trigger = new Subject<int>();
            var work = workRegistry.Register(trigger, new TestWorkerThrowsException());
            IExecutableActionWork result = null;
            work.Executed.Subscribe(w => result = w);

            // Act
            trigger.OnNext(4);

            // Assert
            Assert.Equal(ExecutableWorkState.Error, result.State);
            Assert.Equal(TestWorkerThrowsException.ErrorMessage, result.Error.Message);
        }
    }
}
