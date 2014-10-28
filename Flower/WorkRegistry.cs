using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public sealed class WorkRegistry : IWorkRegistry, IDisposable
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

        public WorkRegistryOptions Options { get; set; }

        public IWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> workerResolver)
        {
            var work = new Work<TInput, TOutput>(this, trigger, workerResolver);
            Add(work);
            if(Options.WorkActivationBehavior == WorkActivationBehavior.RegisterActivated)
            {
                work.Activate();
            }
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

        public void ActivateAllWorks()
        {
            foreach (var work in works.Reverse())
            {
                work.Activate();
            }
        }

        public void SuspendAllWorks()
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

        internal void TriggerCompleted(IWork work)
        {
            Unregister(work);
        }

        internal void TriggerErrored(IWork work, Exception exception)
        {
            Unregister(work);
        }

        internal void Triggered<TInput, TResult>(Work<TInput, TResult> work, TInput input)
        {
            var workRunner = Options.WorkRunnerResolver.Resolve(work);
            var triggeredWork = new TriggeredWork<TInput, TResult>(workRunner, work, input);
            work.TriggeredWorkCreated(triggeredWork);
            triggeredWork.Submit();            
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