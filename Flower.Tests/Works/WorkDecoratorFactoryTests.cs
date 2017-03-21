namespace Flower.Tests.Works
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using Flower.Tests.TestDoubles;
    using Flower.Works;
    using Xunit;

    public class WorkDecoratorFactoryTests
    {
        [Fact]
        public void WorkExecutionLoggingIsPossible()
        {
            // Arrange
            var inputLog = new List<int>();
            var outputLog = new List<int>();
            var logginWorkDecoratorFactory = new LoggingWorkDecoratorFactory(
                input => inputLog.Add(input),
                output => outputLog.Add(output));
            var options = new WorkOptions(
                WorkRegisterMode.Activated,
                workDecoratorFactory: logginWorkDecoratorFactory);
            var workRegistry = new WorkRegistry(options);
            var trigger = new Subject<int>();
            var work = workRegistry.RegisterWorker(trigger, new TestWorkerIntToIntSquared());

            // Act
            trigger.OnNext(3);
            trigger.OnNext(4);
            trigger.OnNext(5);
            trigger.OnCompleted();

            // Assert
            Assert.Equal(WorkState.Completed, work.State);
            Assert.Equal(new[] {3, 4, 5}, inputLog);
            Assert.Equal(new[] {3*3, 4*4, 5*5}, outputLog);
        }
    }

    public class LoggingWorkDecoratorFactory : WorkDecoratorFactory
    {
        private readonly Action<int> logInput;
        private readonly Action<int> logOutput;

        public LoggingWorkDecoratorFactory(Action<int> logInput, Action<int> logOutput)
        {
            this.logInput = logInput;
            this.logOutput = logOutput;
        }
        
        public override IExecutableFuncWork<TInput, TOutput> Decorate<TInput, TOutput>(IExecutableFuncWork<TInput, TOutput> work)
        {
            return new LoggingFuncWork<TInput, TOutput>(work, logInput as Action<TInput>, logOutput as Action<TOutput>);
        }
    }

    public class LoggingFuncWork<TInput, TOutput> : ExecutableWorkDecorator<TInput, TOutput>
    {
        private readonly Action<TInput> logInput;
        private readonly Action<TOutput> logOutput;

        public LoggingFuncWork(IExecutableFuncWork<TInput, TOutput> next, Action<TInput> logInput, Action<TOutput> logOutput)
            : base(next)
        {
            this.logInput = logInput;
            this.logOutput = logOutput;
        }

        public override void Execute()
        {
            logInput(Input);
            base.Execute();
            logOutput(Output);
        }
    }
}
