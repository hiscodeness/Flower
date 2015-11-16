using System;

namespace Flower.Works
{
    internal abstract class WorkRegistration<TInput>
    {
        protected WorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, WorkOptions options)
        {
            WorkRegistry = workRegistry;
            Trigger = trigger;
            Options = options;
        }

        public IWorkRegistry WorkRegistry { get; }
        public IObservable<TInput> Trigger { get; private set; }
        public WorkOptions Options { get; }
    }

    internal class ActionWorkRegistration : WorkRegistration<object>, IActionWorkRegistration
    {
        public ActionWorkRegistration(IWorkRegistry workRegistry, IObservable<object> trigger, Func<IScope<IWorker>> createWorkerScope, WorkOptions options)
            : base(workRegistry, trigger, options)
        {
            CreateWorkerScope = createWorkerScope;
        }

        public Func<IScope<IWorker>> CreateWorkerScope { get; }
    }
    
    internal class ActionWorkRegistration<TInput> : WorkRegistration<TInput>, IActionWorkRegistration<TInput>
    {
        public ActionWorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, Func<IScope<IWorker<TInput>>> createWorkerScope, WorkOptions options)
            : base(workRegistry, trigger, options)
        {
            CreateWorkerScope = createWorkerScope;
        }

        public Func<IScope<IWorker<TInput>>> CreateWorkerScope { get; }
    }

    internal class FuncWorkRegistration<TInput, TOutput> : WorkRegistration<TInput>, IFuncWorkRegistration<TInput, TOutput>
    {
        public FuncWorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope, WorkOptions options)
            : base(workRegistry, trigger, options)
        {
            CreateWorkerScope = createWorkerScope;
        }

        public Func<IScope<IWorker<TInput, TOutput>>> CreateWorkerScope { get; }
    }
}
