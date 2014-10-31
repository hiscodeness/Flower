using System;
using System.Reactive.Linq;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class Work<TInput> : WorkBase<TInput>, IRegisteredWork<TInput>
    {
        private readonly WorkObservables<IWork<TInput>, ITriggeredWork<TInput>> observables;

        public Work(IWorkRegistration<TInput> registration)
            : base(registration)
        {
            Registration = registration;
            observables = new WorkObservables<IWork<TInput>, ITriggeredWork<TInput>>(this);
        }

        new public IWorkRegistration<TInput> Registration { get; private set; }
        public IObservable<ITriggeredWork<TInput>> Triggered { get { return observables.Triggered; } }
        public IObservable<ITriggeredWork<TInput>> Executed { get { return observables.Executed; } }

        protected override void TriggeredWorkCreated(ITriggeredWorkBase triggeredWork)
        {
            observables.TriggeredWorkCreated(triggeredWork as ITriggeredWork<TInput>);
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
            return new TriggeredWork<TInput>(workRunner, this, input);
        }

        public void WorkerErrored(ITriggeredWork<TInput> triggeredWork, Exception error)
        {
            WorkerErrored(error);
        }

        public void WorkerExecuted(ITriggeredWork<TInput> triggeredWork)
        {
            observables.TriggeredWorkExecuted(triggeredWork);
        }
    }
}