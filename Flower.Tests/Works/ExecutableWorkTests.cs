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
            var work = workRegistry.Register(trigger, new TestWorkerIntToIntThrowOnEven());
            IExecutableFuncWork<int, int> result = null;
            work.Executed.Subscribe(w => result = w);

            // Act
            trigger.OnNext(4);

            // Assert
            Assert.Equal(default(int), result.Output);
            Assert.Equal(ExecutableWorkState.Error, result.State);
            Assert.Equal(TestWorkerIntToIntThrowOnEven.ErrorMessage, result.Error.Message);
        }

        [Fact]
        public void WorkerErrorIsIgnoredOnOutput()
        {
            // Arrange
            var options = new WorkRegistryOptions(
                RegisterWorkBehavior.RegisterActivated, workerErrorBehavior: WorkerErrorBehavior.RaiseExecutedAndContinue);
            var workRegistry = new WorkRegistry(options);
            var trigger = new Subject<int>();
            var work = workRegistry.Register(trigger, new TestWorkerIntToIntThrowOnEven());
            var output = new List<int>();
            work.Output.Subscribe(output.Add);

            // Act
            trigger.OnNext(3);
            trigger.OnNext(4);
            trigger.OnNext(5);

            // Assert
            Assert.Equal(new [] {3,5}, output);
        }
    }
}
