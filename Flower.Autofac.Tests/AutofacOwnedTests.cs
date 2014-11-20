using System;
using System.Reactive.Subjects;
using Autofac;
using Autofac.Features.OwnedInstances;
using Flower;
using Flower.Works;
using Xunit;

namespace Flower.Autofac.Tests
{
    public class AutofacOwnedTests
    {
        [Fact]
        public void OwnedInstancesAreExecutedAndDisposed()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<TestWorker>();
            var container = containerBuilder.Build();
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var resolve = container.Resolve<Func<Owned<TestWorker>>>();
            var work = workRegistry.Register(trigger, resolve.Scope());
            IExecutableFuncWork<int, int> executedWork = null;
            work.Executed.Subscribe(w => executedWork = w);

            // Act
            trigger.OnNext(3);

            // Assert
            Assert.NotNull(executedWork);
            Assert.Equal(3+1, executedWork.Output);
            Assert.True((executedWork.WorkerScope.Worker as TestWorker).IsDisposed);
        }
    }

    public sealed class TestWorker : IWorker<int, int>, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public int Execute(int input)
        {
            return input + 1;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
