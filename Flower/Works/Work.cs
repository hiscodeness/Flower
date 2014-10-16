using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Flower.Workers;

namespace Flower.Works
{
    internal class Work<TInput, TOutput> : IWork<TInput, TOutput>
    {
        private IDisposable _triggerSubscription;

        public Work(
            IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> workerResolver)
        {
            WorkRegistry = workRegistry;
            Trigger = trigger;
            WorkerResolver = workerResolver;

            Executed = Observable.Create<ITriggeredWork<TInput, TOutput>>(obs =>
                {
                    if(State == WorkState.Completed ||
                       State == WorkState.WorkerError ||
                       State == WorkState.TriggerError)
                    {
                        obs.OnCompleted();
                        return Disposable.Empty;
                    }

                    _executed += obs.OnNext;
                    _triggerCompleted += obs.OnCompleted;
                    _triggerErrored += obs.OnError;

                    return Disposable.Create(() =>
                        {
                            _executed -= obs.OnNext;
                            _triggerCompleted -= obs.OnCompleted;
                            _triggerErrored -= obs.OnError;
                        });
                });

            Output = Executed.Select(executedWork => executedWork.Output);
        }

        public IWorkRegistry WorkRegistry { get; private set; }
        public WorkState State { get; private set; }

        IWorkerResolver<TInput> IWork<TInput>.WorkerResolver
        {
            get { throw new InvalidOperationException(); }
        }

        IObservable<object> IWork.Trigger
        {
            get { return Trigger.Select(input => input as object); }
        }

        IObservable<ITriggeredWork<TInput>> IWork<TInput>.Executed
        {
            get { throw new InvalidOperationException(); }
        }

        IWorkerResolver IWork.WorkerResolver
        {
            get { throw new InvalidOperationException(); }
        }

        IObservable<ITriggeredWorkBase> IWork.Executed
        {
            get { return Executed; }
        }

        public IObservable<TInput> Trigger { get; private set; }
        public IWorkerResolver<TInput, TOutput> WorkerResolver { get; private set; }
        public IObservable<ITriggeredWork<TInput, TOutput>> Executed { get; private set; }
        public IObservable<TOutput> Output { get; private set; }

        public void Activate()
        {
            State = WorkState.Active;
            _triggerSubscription = Trigger.Subscribe(TriggerOnNext,
                                                     TriggerOnError,
                                                     TriggerOnCompleted);
        }

        public void Suspend()
        {
            if(_triggerSubscription == null) return;

            State = WorkState.Suspended;
            _triggerSubscription.Dispose();
            _triggerSubscription = null;
        }

        internal void TriggeredWorkErrored(
            ITriggeredWork<TInput, TOutput> triggeredWork, Exception error)
        {
            switch(WorkRegistry.WorkerErrorBehavior)
            {
                case WorkerErrorBehavior.Ignore:
                    // Eats exception
                    //Log.Warning("Continue on worker error: {0}.", error);
                    break;
                case WorkerErrorBehavior.Complete:
                    ((WorkRegistry)WorkRegistry).Unregister(this);
                    State = WorkState.WorkerError;
                    break;
                case WorkerErrorBehavior.Throw:
                    ((WorkRegistry)WorkRegistry).Unregister(this);
                    State = WorkState.WorkerError;
                    throw error;
            }
        }

        internal void TriggeredWorkExecuted(ITriggeredWork<TInput, TOutput> triggeredWork)
        {
            OnWorkExecuted(triggeredWork);
        }

        private void OnWorkCompleted()
        {
            var handler = _triggerCompleted;
            if(handler != null)
            {
                handler();
            }
        }

        private void OnWorkExecuted(ITriggeredWork<TInput, TOutput> triggeredWork)
        {
            var handler = _executed;
            if(handler != null)
            {
                handler(triggeredWork);
            }
        }

        private void TriggerOnCompleted()
        {
            ((WorkRegistry)WorkRegistry).TriggerCompleted(this);
            State = WorkState.Completed;
            OnWorkCompleted();
        }

        private void TriggerOnError(Exception exception)
        {
            ((WorkRegistry)WorkRegistry).TriggerErrored(this, exception);
            State = WorkState.TriggerError;
        }

        private void TriggerOnNext(TInput input)
        {
            ((WorkRegistry)WorkRegistry).Triggered(this, input);
        }

        private event Action<ITriggeredWork<TInput, TOutput>> _executed;
        private event Action _triggerCompleted;
        private event Action<Exception> _triggerErrored;
    }
}