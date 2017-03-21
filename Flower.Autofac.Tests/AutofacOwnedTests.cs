namespace Flower.Autofac.Tests
{
    using System;
    using System.Globalization;
    using System.Reactive.Subjects;
    using Flower.Works;
    using global::Autofac;
    using global::Autofac.Features.OwnedInstances;
    using Xunit;

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
            var factory = container.Resolve<Func<Owned<TestWorkerIntToStringDisposable>>>();
            var work = workRegistry.RegisterFactory(trigger, factory.Scope());
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
            var work1 = workRegistry.RegisterFactory(trigger, intToString.Scope());
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

        [Fact]
        public void OwnedInstancesWithNoOutputCanBePiped()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<TestWorkerIntToStringDisposable>();
            containerBuilder.RegisterType<TestWorkerStringDisposable>();
            var container = containerBuilder.Build();
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var intToString = container.Resolve<Func<Owned<TestWorkerIntToStringDisposable>>>();
            var stringToInt = container.Resolve<Func<Owned<TestWorkerStringDisposable>>>();
            var work1 = workRegistry.RegisterFactory(trigger, intToString.Scope());
            var work2 = work1.Pipe(stringToInt.Scope());
            IExecutableActionWork<string> executedWork = null;
            work2.Executed.Subscribe(w => executedWork = w);

            // Act
            trigger.OnNext(3);

            // Assert
            Assert.NotNull(executedWork);
            Assert.Equal(3, (executedWork.WorkerScope.Worker as TestWorkerStringDisposable).Result);
            Assert.True((executedWork.WorkerScope.Worker as TestWorkerStringDisposable).IsDisposed);
        }

        [Fact]
        public void OwnedInstancesWithNoInputOrOutputCanBePiped()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<TestWorkerIntToStringDisposable>();
            containerBuilder.RegisterType<TestWorkerDisposable>();
            var container = containerBuilder.Build();
            var workRegistry = new WorkRegistry();
            var trigger = new Subject<int>();
            var intToString = container.Resolve<Func<Owned<TestWorkerIntToStringDisposable>>>();
            var worker = container.Resolve<Func<Owned<TestWorkerDisposable>>>();
            var work1 = workRegistry.RegisterFactory(trigger, intToString.Scope());
            var work2 = work1.Pipe(worker.Scope());
            IExecutableActionWork executedWork = null;
            work2.Executed.Subscribe(w => executedWork = w);

            // Act
            trigger.OnNext(3);

            // Assert
            Assert.NotNull(executedWork);
            Assert.True((executedWork.WorkerScope.Worker as TestWorkerDisposable).IsExecuted);
            Assert.True((executedWork.WorkerScope.Worker as TestWorkerDisposable).IsDisposed);
        }
    }
}
