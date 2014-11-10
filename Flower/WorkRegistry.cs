using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public sealed class WorkRegistry : IWorkRegistry, IActivatable, ISuspendable, IDisposable
    {
        private bool isDisposed;
        private readonly BlockingCollection<IWork> works = new BlockingCollection<IWork>();

        public WorkRegistry(WorkRegistryOptions options = null)
        {
            Options = options ?? WorkRegistryOptions.Default;
        }

        public IEnumerable<IWork> Works
        {
            get { return isDisposed ? Enumerable.Empty<IWork>() : works; }
        }

        public WorkRegistryOptions Options { get; private set; }

        public IActionWork Register<TInput>(IObservable<TInput> trigger, IWorkerResolver workerResolver)
        {
            var work = new ActionWork(new ActionWorkRegistration(this, trigger.Select(input => (object)input), workerResolver));
            Register(work);
            return work;
        }
        
        public IActionWork<TInput> Register<TInput>(IObservable<TInput> trigger, IWorkerResolver<TInput> workerResolver)
        {
            var work = new ActionWork<TInput>(new ActionWorkRegistration<TInput>(this, trigger, workerResolver));
            Register(work);
            return work;
        }

        public IFuncWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> workerResolver)
        {
            var work = new FuncWork<TInput, TOutput>(new FuncWorkRegistration<TInput, TOutput>(this, trigger, workerResolver));
            Register(work);
            return work;
        }

        public void Unregister(IWork work)
        {
            if (work == null) throw new ArgumentNullException("work");

            if (!works.Contains(work))
            {
                throw new InvalidOperationException(
                    "Cannot unregister work that is not contained in this work registry.");
            }

            work.Suspend();
            Remove(work);
            work.Unregister();
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

        public void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;
            foreach (var work in works.ToList())
            {
                Unregister(work);
            }

            works.Dispose();
        }

        private void Register(IWork work)
        {
            Add(work);
            if (Options.RegisterWorkBehavior == RegisterWorkBehavior.RegisterActivated)
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
            works.TryTake(out work);
        }
    }
}