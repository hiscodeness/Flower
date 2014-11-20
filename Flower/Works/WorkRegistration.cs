using System;
using Flower.Workers;

namespace Flower.Works
{
    internal abstract class WorkRegistration<TInput>
    {
        protected WorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, RegisterOptions options)
        {
            WorkRegistry = workRegistry;
            Trigger = trigger;
            Options = options;
        }

        public IWorkRegistry WorkRegistry { get; private set; }
        public IObservable<TInput> Trigger { get; private set; }
        public RegisterOptions Options { get; private set; }
    }

    internal class ActionWorkRegistration : WorkRegistration<object>, IActionWorkRegistration
    {
        public ActionWorkRegistration(IWorkRegistry workRegistry, IObservable<object> trigger, Func<IScope<IWorker>> createWorkerScope, RegisterOptions options)
            : base(workRegistry, trigger, options)
        {
            CreateWorkerScope = createWorkerScope;
        }

        public Func<IScope<IWorker>> CreateWorkerScope { get; private set; }
    }
    
    internal class ActionWorkRegistration<TInput> : WorkRegistration<TInput>, IActionWorkRegistration<TInput>
    {
        public ActionWorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, Func<IScope<IWorker<TInput>>> createWorkerScope, RegisterOptions options)
            : base(workRegistry, trigger, options)
        {
            CreateWorkerScope = createWorkerScope;
        }

        public Func<IScope<IWorker<TInput>>> CreateWorkerScope { get; private set; }
    }

    internal class FuncWorkRegistration<TInput, TOutput> : WorkRegistration<TInput>, IFuncWorkRegistration<TInput, TOutput>
    {
        public FuncWorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope, RegisterOptions options)
            : base(workRegistry, trigger, options)
        {
            CreateWorkerScope = createWorkerScope;
        }

        public Func<IScope<IWorker<TInput, TOutput>>> CreateWorkerScope { get; private set; }
    }
}