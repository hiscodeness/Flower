namespace Flower.Tests.TestContexts
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Subjects;
    using Flower.Tests.TestDoubles;
    using Flower.Works;

    public sealed class IntToIntWorkerThrowOnEvenContext : IDisposable
    {
        private readonly Subject<int> trigger;

        public IntToIntWorkerThrowOnEvenContext(WorkerErrorMode mode)
        {
            var options = new WorkOptions(WorkRegisterMode.Activated, workerErrorMode: mode);
            var workRegistry = new WorkRegistry(options);
            trigger = new Subject<int>();
            Work = workRegistry.RegisterWorker(trigger, new TestWorkerIntToIntThrowOnEven());
            Triggered = new List<ITriggeredFuncWork<int, int>>();
            Errored = new List<IExecutableFuncWork<int, int>>();
            Executed = new List<IExecutableFuncWork<int, int>>();
            Output = new List<int>();
            Work.Triggered.Subscribe(Triggered.Add);
            Work.Errored.Subscribe(Errored.Add);
            Work.Executed.Subscribe(Executed.Add);
            Work.Output.Subscribe(Output.Add);
        }

        public IFuncWork<int, int> Work { get; }
        public List<ITriggeredFuncWork<int, int>> Triggered { get; }
        public List<IExecutableFuncWork<int, int>> Executed { get; }
        public List<IExecutableFuncWork<int, int>> Errored { get; }
        public List<int> Output { get; }

        public void Trigger(params int[] values)
        {
            foreach (var value in values)
            {
                trigger.OnNext(value);
            }
        }

        public void Dispose()
        {
            trigger.Dispose();
        }
    }
}
