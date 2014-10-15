using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

    public class WorkRegistry : IWorkRegistry
    {
        private readonly BlockingCollection<IWork> _works = new BlockingCollection<IWork>();

        public WorkRegistry(bool activateWorkWhenRegistered = true, WorkerErrorBehavior workerErrorBehavior = WorkerErrorBehavior.Throw)
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
            get { return _works; }
        }

        public WorkerErrorBehavior WorkerErrorBehavior { get; private set; }

        public bool ActivateWorkWhenRegistered { get; private set; }

        public void ActivateAllWorks()
        {
            foreach(var work in _works.Reverse())
            {
                work.Activate();
            }
        }

        public IWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> workerResolver)
        {
            return Add(new Work<TInput, TOutput>(this, trigger, workerResolver));
        }

        public void SuspendAllWorks()
        {
            foreach(var work in _works)
            {
                work.Suspend();
            }
        }

        public void Unregister(IWork work)
        {
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
            if(work == null) throw new ArgumentNullException("work");

            if(_works.TryAdd(work) && ActivateWorkWhenRegistered)
            {
                work.Activate();
            }

            return work;
        }

        private TWork Remove<TWork>(TWork work) where TWork : IWork
        {
            if(work == null) throw new ArgumentNullException("work");

            if(!_works.Contains(work))
            {
                throw new InvalidOperationException(
                    "Cannot unregister work that is not contained in this work registry.");
            }

            IWork removedWork;
            if(_works.TryTake(out removedWork))
            {
                work.Suspend();
            }

            return work;
        }
    }
}