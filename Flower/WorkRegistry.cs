using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Flower.Works;

namespace Flower
{
    public sealed class WorkRegistry : IWorkRegistry, IActivatable, ISuspendable
    {
        private readonly IList<IWork> works = new List<IWork>();

        public WorkRegistry(RegisterOptions options = null)
        {
            DefaultOptions = options ?? RegisterOptions.Default;
        }

        public IEnumerable<IWork> Works => works;

        public RegisterOptions DefaultOptions { get; }

        public IActionWork Register<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker>> createWorkerScope,
            RegisterOptions options = null)
        {
            options = CreateRegisterOptions(options);
            var registration = CreateRegistration(trigger, createWorkerScope, options);
            var work = new ActionWork(registration);
            Register(work);
            return work;
        }

        public IActionWork<TInput> Register<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput>>> createWorkerScope,
            RegisterOptions options = null)
        {
            options = CreateRegisterOptions(options);
            var registration = CreateRegistration(trigger, createWorkerScope, options);
            var work = new ActionWork<TInput>(registration);
            Register(work);
            return work;
        }
        
        public IFuncWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope,
            RegisterOptions options = null)
        {
            options = CreateRegisterOptions(options);
            var registration = CreateRegistration(trigger, createWorkerScope, options);
            var work = new FuncWork<TInput, TOutput>(registration);
            Register(work);
            return work;
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