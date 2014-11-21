using System;
using System.Globalization;
using System.Reactive.Subjects;
using Autofac;
using Autofac.Features.OwnedInstances;
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
            containerBuilder.RegisterType<TestWorkerIntToStringDisposable>();
            var container = containerBuilder.Build();
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var resolve = container.Resolve<Func<Owned<TestWorkerIntToStringDisposable>>>();
            var work = workRegistry.Register(trigger, resolve.Scope());
            IExecutableFuncWork<int, string> executedWork = null;
            work.Executed.Subscribe(w => executedWork = w);

            // Act
            trigger.OnNext(3);

            // Assert
            Assert.NotNull(executedWork);
            Assert.Equal(3.ToString(CultureInfo.InvariantCulture), executedWork.Output);
            Assert.True((executedWork.WorkerScope.Worker as TestWorkerIntToStringDisposable).IsDisposed);
        }

        [Fact]
        public void OwnedInstancesCanBePiped()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<TestWorkerIntToStringDisposable>();
            containerBuilder.RegisterType<TestWorkerStringToIntDisposable>();
            var container = containerBuilder.Build();
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var intToString = container.Resolve<Func<Owned<TestWorkerIntToStringDisposable>>>();
            var stringToInt = container.Resolve<Func<Owned<TestWorkerStringToIntDisposable>>>();
            var work1 = workRegistry.Register(trigger, intToString.Scope());
            var work2 = work1.Pipe(stringToInt.Scope());
            IExecutableFuncWork<string, int> executedWork = null;
            work2.Executed.Subscribe(w => executedWork = w);

            // Act
            trigger.OnNext(3);

            // Assert
            Assert.NotNull(executedWork);
            Assert.Equal(3, executedWork.Output);
            Assert.True((executedWork.WorkerScope.Worker as TestWorkerStringToIntDisposable).IsDisposed);
        }
    }

    public sealed class TestWorkerIntToStringDisposable : IWorker<int, string>, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public string Execute(int input)
        {
            return input.ToString(CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    public sealed class TestWorkerStringToIntDisposable : IWorker<string, int>, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public int Execute(string input)
        {
            return int.Parse(input, CultureInfo.InvariantCulture);
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
