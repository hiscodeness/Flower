using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Flower.WorkRunners;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public enum WorkerErrorBehavior
    {
        Throw,
        Ignore,
        Complete
    }

    public sealed class WorkRegistry : IWorkRegistry, IDisposable
    {
        private readonly BlockingCollection<IWork> works = new BlockingCollection<IWork>();

        public WorkRegistry(
            bool activateWorkWhenRegistered = true,
            WorkerErrorBehavior workerErrorBehavior = WorkerErrorBehavior.Throw)
        {
            ActivateWorkWhenRegistered = activateWorkWhenRegistered;
            WorkerErrorBehavior = workerErrorBehavior;
            FallbackWorkRunner = new ImmediateWorkRunner();
            ResolveWorkRunner = _ => FallbackWorkRunner;
        }

        public IWorkRunner FallbackWorkRunner { get; set; }
        public Func<IWork, IWorkRunner> ResolveWorkRunner { get; set; }

        public IEnumerable<IWork> Works
        {
            get { return works; }
        }

        public WorkerErrorBehavior WorkerErrorBehavior { get; private set; }

        public bool ActivateWorkWhenRegistered { get; private set; }

        public void ActivateAllWorks()
        {
            foreach(var work in works.Reverse())
            {
                work.Activate();
            }
        }

        public void Dispose()
        {
            foreach(var work in works.ToList())
            {
                Unregister(work);
            }

            works.Dispose();
        }

        public IWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> workerResolver)
        {
            return Add(new Work<TInput, TOutput>(this, trigger, workerResolver));
        }

        public void SuspendAllWorks()
        {
            foreach(var work in works)
            {
                work.Suspend();
            }
        }

        public void Unregister(IWork work)
        {
            if(work == null) throw new ArgumentNullException("work");

            if(!works.Contains(work))
            {
                throw new InvalidOperationException(
                    "Cannot unregister work that is not contained in this work registry.");
            }

            Remove(work);
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
            var workRunner = ResolveWorkRunner(work) ?? FallbackWorkRunner;
            var triggeredWork = new TriggeredWork<TInput, TResult>(workRunner, work, input);
            workRunner.Submit(triggeredWork);
        }

        private TWork Add<TWork>(TWork work) where TWork : IWork
        {
            works.Add(work);
            if(ActivateWorkWhenRegistered)
            {
                work.Activate();
            }

            return work;
        }

        private void Remove(IWork work)
        {
            work.Suspend();
            works.TryTake(out work);
        }
    }
}