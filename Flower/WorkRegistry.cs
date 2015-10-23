using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Flower.Works;

namespace Flower
{
    using Flower.Workers;

    public sealed class WorkRegistry : IWorkRegistry, IActivatable, ISuspendable
    {
        private readonly IList<IWork> works = new List<IWork>();

        public WorkRegistry(RegisterOptions options = null)
        {
            DefaultOptions = options ?? RegisterOptions.Default;
        }

        public IEnumerable<IWork> Works => works;

        public RegisterOptions DefaultOptions { get; }

        public IActionWork RegisterMethod<TInput>(
           IObservable<TInput> trigger,
           Action worker,
           RegisterOptions options = null)
        {
            return RegisterFactory(trigger, () => WorkerScope.FromInstance(new Worker(worker)), options);
        }

        public IActionWork<TInput> RegisterMethod<TInput>(
            IObservable<TInput> trigger,
            Action<TInput> worker,
            RegisterOptions options = null)
        {
            return RegisterFactory(trigger, WorkerScope.FromInstance(new Worker<TInput>(worker)), options);
        }

        public IFuncWork<TInput, TOutput> RegisterMethod<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<TInput, TOutput> worker,
            RegisterOptions options = null)
        {
            return RegisterFactory(trigger, WorkerScope.FromInstance(new Worker<TInput, TOutput>(worker)), options);
        }

        public IActionWork RegisterWorker<TInput>(
            IObservable<TInput> trigger,
            IWorker worker,
            RegisterOptions options = null)
        {
            return RegisterFactory(trigger, () => WorkerScope.FromInstance(worker), options);
        }

        public IActionWork<TInput> RegisterWorker<TInput>(
            IObservable<TInput> trigger,
            IWorker<TInput> worker,
            RegisterOptions options = null)
        {
            return RegisterFactory(trigger, WorkerScope.FromInstance(worker), options);
        }

        public IFuncWork<TInput, TOutput> RegisterWorker<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorker<TInput, TOutput> worker,
            RegisterOptions options = null)
        {
            return RegisterFactory(trigger, WorkerScope.FromInstance(worker), options);
        }

        public IActionWork RegisterResolver<TInput>(
           IObservable<TInput> trigger,
           IWorkerResolver resolver,
           RegisterOptions options = null)
        {
            return RegisterFactory(trigger, () => WorkerScope.FromResolver(resolver), options);
        }

        public IActionWork<TInput> RegisterResolver<TInput>(
           IObservable<TInput> trigger,
           IWorkerResolver<TInput> resolver,
           RegisterOptions options = null)
        {
            return RegisterFactory(trigger, () => WorkerScope.FromResolver(resolver), options);
        }

        public IFuncWork<TInput, TOutput> RegisterResolver<TInput, TOutput>(
           IObservable<TInput> trigger,
           IWorkerResolver<TInput, TOutput> resolver,
           RegisterOptions options = null)
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
            foreach(var work in works)
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
            RegisterOptions options = null)
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
            RegisterOptions options = null)
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
            RegisterOptions options = null)
        {
            options = CreateRegisterOptions(options);
            var registration = CreateRegistration(trigger, factory, options);
            var work = new FuncWork<TInput, TOutput>(registration);
            Register(work);
            return work;
        }

        private RegisterOptions CreateRegisterOptions(RegisterOptions options)
        {
            return options ?? new RegisterOptions(DefaultOptions);
        }

        private ActionWorkRegistration CreateRegistration<TInput>(
            IObservable<TInput> trigger, Func<IScope<IWorker>> createWorkerScope, RegisterOptions options)
        {
            return new ActionWorkRegistration(
                this, trigger.Select(input => (object)input), createWorkerScope, options);
        }

        private ActionWorkRegistration<TInput> CreateRegistration<TInput>(
            IObservable<TInput> trigger, Func<IScope<IWorker<TInput>>> createWorkerScope, RegisterOptions options)
        {
            return new ActionWorkRegistration<TInput>(this, trigger, createWorkerScope, options);
        }

        private FuncWorkRegistration<TInput, TOutput> CreateRegistration<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope,
            RegisterOptions options)
        {
            return new FuncWorkRegistration<TInput, TOutput>(this, trigger, createWorkerScope, options);
        }

        private void Register(IWork work)
        {
            Add(work);
            if (DefaultOptions.RegisterWorkBehavior == RegisterWorkBehavior.RegisterActivated)
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