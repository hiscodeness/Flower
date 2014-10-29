using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public sealed class WorkRegistry : IWorkRegistry
    {
        private bool isDisposed;
        private readonly BlockingCollection<IWorkBase> works = new BlockingCollection<IWorkBase>();

        public WorkRegistry(WorkRegistryOptions options = null)
        {
            Options = options ?? WorkRegistryOptions.Default;
        }

        public IEnumerable<IWorkBase> Works
        {
            get { return isDisposed ? Enumerable.Empty<IWorkBase>() : works; }
        }

        public WorkRegistryOptions Options { get; private set; }

        public IWork<TInput> Register<TInput>(
           IObservable<TInput> trigger,
           IWorkerResolver<TInput> workerResolver)
        {
            var work = new Work<TInput>(new WorkRegistration<TInput>(this, trigger, workerResolver));
            Add(work);
            if (Options.RegisterWorkBehavior == RegisterWorkBehavior.RegisterActivated)
            {
                work.Activate();
            }
            return work;
        }

        public IWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> workerResolver)
        {
            var work = new Work<TInput, TOutput>(new WorkRegistration<TInput, TOutput>(this, trigger, workerResolver));
            Add(work);
            if(Options.RegisterWorkBehavior == RegisterWorkBehavior.RegisterActivated)
            {
                work.Activate();
            }
            return work;
        }

        public void Unregister(IWorkBase work)
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

        internal void TriggerCompleted(IWorkBase work)
        {
            Unregister(work);
        }

        internal void TriggerErrored(IWorkBase work, Exception exception)
        {
            Unregister(work);
        }
        internal void Triggered<TInput>(Work<TInput> work, TInput input)
        {
            var workRunner = Options.WorkRunnerResolver.Resolve(work);
            var triggeredWork = new TriggeredWork<TInput>(workRunner, work, input);
            work.TriggeredWorkCreated(triggeredWork);
            triggeredWork.Submit();
        }

        internal void Triggered<TInput, TResult>(Work<TInput, TResult> work, TInput input)
        {
            var workRunner = Options.WorkRunnerResolver.Resolve(work);
            var triggeredWork = new TriggeredWork<TInput, TResult>(workRunner, work, input);
            work.TriggeredWorkCreated(triggeredWork);
            triggeredWork.Submit();            
        }

        private void Add<TWork>(TWork work) where TWork : IWorkBase
        {
            works.Add(work);
        }

        private void Remove(IWorkBase work)
        {
            works.TryTake(out work);
        }
    }
}