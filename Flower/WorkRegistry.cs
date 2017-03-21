namespace Flower
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using Flower.Workers;
    using Flower.Works;

    public sealed class WorkRegistry : IWorkRegistry, IActivatable, ISuspendable
    {
        private readonly IList<IWork> works = new List<IWork>();

        public WorkRegistry(WorkOptions options = null)
        {
            Options = options ?? WorkOptions.Default;
        }

        public IEnumerable<IWork> Works => works;

        public WorkOptions Options { get; }

        public IActionWork RegisterMethod<TInput>(
            IObservable<TInput> trigger,
            Func<Task> worker,
            WorkOptions options = null)
        {
            return RegisterFactory(trigger, () => WorkerScope.FromInstance(new Worker(worker)), options);
        }

        public IActionWork<TInput> RegisterMethod<TInput>(
            IObservable<TInput> trigger,
            Func<TInput, Task> worker,
            WorkOptions options = null)
        {
            return RegisterFactory(trigger, WorkerScope.FromInstance(new Worker<TInput>(worker)), options);
        }

        public IFuncWork<TInput, TOutput> RegisterMethod<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<TInput, Task<TOutput>> worker,
            WorkOptions options = null)
        {
            return RegisterFactory(trigger, WorkerScope.FromInstance(new Worker<TInput, TOutput>(worker)), options);
        }

        public IActionWork RegisterWorker<TInput>(
            IObservable<TInput> trigger,
            IWorker worker,
            WorkOptions options = null)
        {
            return RegisterFactory(trigger, () => WorkerScope.FromInstance(worker), options);
        }

        public IActionWork<TInput> RegisterWorker<TInput>(
            IObservable<TInput> trigger,
            IWorker<TInput> worker,
            WorkOptions options = null)
        {
            return RegisterFactory(trigger, WorkerScope.FromInstance(worker), options);
        }

        public IFuncWork<TInput, TOutput> RegisterWorker<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorker<TInput, TOutput> worker,
            WorkOptions options = null)
        {
            return RegisterFactory(trigger, WorkerScope.FromInstance(worker), options);
        }

        public IActionWork RegisterResolver<TInput>(
            IObservable<TInput> trigger,
            IWorkerResolver resolver,
            WorkOptions options = null)
        {
            return RegisterFactory(trigger, () => WorkerScope.FromResolver(resolver), options);
        }

        public IActionWork<TInput> RegisterResolver<TInput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput> resolver,
            WorkOptions options = null)
        {
            return RegisterFactory(trigger, () => WorkerScope.FromResolver(resolver), options);
        }

        public IFuncWork<TInput, TOutput> RegisterResolver<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> resolver,
            WorkOptions options = null)
        {
            return RegisterFactory(trigger, () => WorkerScope.FromResolver(resolver), options);
        }

        public void Activate()
        {
            foreach (var work in works.Reverse())
            {
                work.Activate();
            }
        }

        public void Suspend()
        {
            foreach (var work in works)
            {
                work.Suspend();
            }
        }

        public void Complete(IWork work)
        {
            if (work == null) throw new ArgumentNullException(nameof(work));

            if (!works.Contains(work))
            {
                throw new InvalidOperationException(
                    "Cannot complete work that is not contained in this work registry.");
            }

            work.Suspend();
            Remove(work);
            work.Complete();
        }

        public void CompleteAll()
        {
            foreach (var work in works.Reverse().ToList())
            {
                Complete(work);
            }
        }

        public IActionWork RegisterFactory<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker>> factory,
            WorkOptions options = null)
        {
            options = CreateRegisterOptions(options);
            var registration = CreateRegistration(trigger, factory, options);
            var work = new ActionWork(registration);
            Register(work);
            return work;
        }

        public IActionWork<TInput> RegisterFactory<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput>>> factory,
            WorkOptions options = null)
        {
            options = CreateRegisterOptions(options);
            var registration = CreateRegistration(trigger, factory, options);
            var work = new ActionWork<TInput>(registration);
            Register(work);
            return work;
        }

        public IFuncWork<TInput, TOutput> RegisterFactory<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput, TOutput>>> factory,
            WorkOptions options = null)
        {
            options = CreateRegisterOptions(options);
            var registration = CreateRegistration(trigger, factory, options);
            var work = new FuncWork<TInput, TOutput>(registration);
            Register(work);
            return work;
        }

        private WorkOptions CreateRegisterOptions(WorkOptions options)
        {
            return options ?? new WorkOptions(Options);
        }

        private ActionWorkRegistration CreateRegistration<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker>> createWorkerScope,
            WorkOptions options)
        {
            return new ActionWorkRegistration(
                this,
                trigger.Select(input => (object) input),
                createWorkerScope,
                options);
        }

        private ActionWorkRegistration<TInput> CreateRegistration<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput>>> createWorkerScope,
            WorkOptions options)
        {
            return new ActionWorkRegistration<TInput>(this, trigger, createWorkerScope, options);
        }

        private FuncWorkRegistration<TInput, TOutput> CreateRegistration<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope,
            WorkOptions options)
        {
            return new FuncWorkRegistration<TInput, TOutput>(this, trigger, createWorkerScope, options);
        }

        private void Register(IWork work)
        {
            Add(work);
            if (Options.WorkRegisterMode == WorkRegisterMode.Activated)
            {
                work.Activate();
            }
        }

        private void Add<TWork>(TWork work) where TWork : IWork
        {
            works.Add(work);
        }

        private void Remove(IWork work)
        {
            works.Remove(work);
        }
    }
}
