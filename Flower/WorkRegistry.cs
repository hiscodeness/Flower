using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public sealed class WorkRegistry : IWorkRegistry, IActivatable, ISuspendable
    {
        private readonly IList<IWork> works = new List<IWork>();

        public WorkRegistry(WorkRegistryOptions options = null)
        {
            DefaultOptions = options ?? WorkRegistryOptions.Default;
        }

        public IEnumerable<IWork> Works
        {
            get { return works; }
        }

        public WorkRegistryOptions DefaultOptions { get; private set; }

        public IActionWork Register<TInput>(IObservable<TInput> trigger, Func<IScope<IWorker>> createWorkerScope)
        {
            var work = new ActionWork(new ActionWorkRegistration(this, trigger.Select(input => (object)input), createWorkerScope));
            Register(work);
            return work;
        }

        public IActionWork<TInput> Register<TInput>(IObservable<TInput> trigger, Func<IScope<IWorker<TInput>>> createWorkerScope)
        {
            var work = new ActionWork<TInput>(new ActionWorkRegistration<TInput>(this, trigger, createWorkerScope));
            Register(work);
            return work;
        }

        public IFuncWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput, TOutput>>> createWorkerScope)
        {
            var work = new FuncWork<TInput, TOutput>(new FuncWorkRegistration<TInput, TOutput>(this, trigger, createWorkerScope));
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
            if (work == null) throw new ArgumentNullException("work");

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