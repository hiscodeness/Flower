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
        public ActionWorkRegistration(IWorkRegistry workRegistry, IObservable<object> trigger, IWorkerResolver workerResolver)
            : base(workRegistry, trigger)
        {
            WorkerResolver = workerResolver;
        }

        public IWorkerResolver WorkerResolver { get; private set; }
    }
    
    internal class ActionWorkRegistration<TInput> : WorkRegistration<TInput>, IActionWorkRegistration<TInput>
    {
        public ActionWorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, IWorkerResolver<TInput> workerResolver)
            : base(workRegistry, trigger)
        {
            WorkerResolver = workerResolver;
        }

        public IWorkerResolver<TInput> WorkerResolver { get; private set; }
    }

    internal class FuncWorkRegistration<TInput, TOutput> : WorkRegistration<TInput>, IFuncWorkRegistration<TInput, TOutput>
    {
        public FuncWorkRegistration(
            IWorkRegistry workRegistry, IObservable<TInput> trigger, IWorkerResolver<TInput, TOutput> workerResolver)
            : base(workRegistry, trigger)
        {
            WorkerResolver = workerResolver;
        }

        public IWorkerResolver<TInput, TOutput> WorkerResolver { get; private set; }
    }
}