using System;
using System.Reactive.Linq;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class Work<TInput, TOutput> : WorkBase<TInput>, IRegisteredWork<TInput, TOutput>
    {
        private readonly WorkObservables<IWork<TInput, TOutput>, ITriggeredWork<TInput, TOutput>> observables;

        public Work(IWorkRegistration<TInput, TOutput> registration) : base(registration)
        {
            Registration = registration;
            observables = new WorkObservables<IWork<TInput, TOutput>, ITriggeredWork<TInput, TOutput>>(this);
            Output = Executed.Select(executedWork => executedWork.Output);
        }

        new public IWorkRegistration<TInput, TOutput> Registration { get; private set; }
        public IObservable<ITriggeredWork<TInput, TOutput>> Triggered { get { return observables.Triggered; } }
        public IObservable<ITriggeredWork<TInput, TOutput>> Executed { get { return observables.Executed; } }
        public IObservable<TOutput> Output { get; private set; }

        protected override void TriggeredWorkCreated(ITriggeredWorkBase triggeredWork)
        {
            observables.TriggeredWorkCreated(triggeredWork as ITriggeredWork<TInput, TOutput>);
        }

        protected override void TriggerErrored(Exception exception)
        {
            observables.OnTriggerErrored(exception);
        }

        protected override void TriggerCompleted()
        {
            observables.OnTriggerCompleted();
        }

        protected override ITriggeredWorkBase CreateTriggeredWork(IWorkRunner workRunner, TInput input)
        {
            return new TriggeredWork<TInput, TOutput>(workRunner, this, input);
        }

        public void WorkerErrored(ITriggeredWork<TInput, TOutput> triggeredWork, Exception error)
        {
            WorkerErrored(error);
        }

        public void WorkerExecuted(ITriggeredWork<TInput, TOutput> triggeredWork)
        {
            observables.TriggeredWorkExecuted(triggeredWork);
        }
    }
}