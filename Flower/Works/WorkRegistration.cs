using System;
using Flower.Workers;

namespace Flower.Works
{
    internal abstract class WorkRegistration<TInput>
    {
        protected WorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger)
        {
            WorkRegistry = workRegistry;
            Trigger = trigger;
        }

        public IWorkRegistry WorkRegistry { get; private set; }
        public WorkRegistryOptions Options { get { return WorkRegistry.Options; } }
        public IObservable<TInput> Trigger { get; private set; }
    }

    internal class ActionWorkRegistration : WorkRegistration<object>, IActionWorkRegistration
    {
        public ActionWorkRegistration(IWorkRegistry workRegistry, IObservable<object> trigger, Func<IScope<IWorker>> createWorkerScope)
            : base(workRegistry, trigger)
        {
            CreateWorkerScope = createWorkerScope;
        }

        public Func<IScope<IWorker>> CreateWorkerScope { get; private set; }
    }
    
    internal class ActionWorkRegistration<TInput> : WorkRegistration<TInput>, IActionWorkRegistration<TInput>
    {
        public ActionWorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, Func<IScope<IWorker<TInput>>> createWorkerScope)
            : base(workRegistry, trigger)
        {
            CreateWorkerScope = createWorkerScope;
        }

        public Func<IScope<IWorker<TInput>>> CreateWorkerScope { get; private set; }
    }

    internal class FuncWorkRegistration<TInput, TOutput> : WorkRegistration<TInput>, IFuncWorkRegistration<TInput, TOutput>
    {
        public FuncWorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope)
            : base(workRegistry, trigger)
        {
            CreateWorkerScope = createWorkerScope;
        }

        public Func<IScope<IWorker<TInput, TOutput>>> CreateWorkerScope { get; private set; }
    }
}