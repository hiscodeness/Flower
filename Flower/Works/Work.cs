using System;
using System.Reactive.Linq;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class Work : WorkBase<object>, IRegisteredWork
    {
        private readonly WorkObservables<IWork, ITriggeredWork> observables;

        public Work(IWorkRegistration registration)
            : base(registration)
        {
            Registration = registration;
            observables = new WorkObservables<IWork, ITriggeredWork>(this);
        }

        new public IWorkRegistration Registration { get; private set; }
        public IObservable<ITriggeredWork> Triggered { get { return observables.Triggered; } }
        public IObservable<ITriggeredWork> Executed { get { return observables.Executed; } }

        protected override void TriggeredWorkCreated(ITriggeredWorkBase triggeredWork)
        {
            observables.TriggeredWorkCreated(triggeredWork as ITriggeredWork);
        }

        protected override void TriggerErrored(Exception exception)
        {
            observables.OnTriggerErrored(exception);
        }

        protected override void TriggerCompleted()
        {
            observables.OnTriggerCompleted();
        }

        protected override ITriggeredWorkBase CreateTriggeredWork(IWorkRunner workRunner, object input)
        {
            return new TriggeredWork(workRunner, this, input);
        }

        public void WorkerErrored(ITriggeredWork triggeredWork, Exception error)
        {
            WorkerErrored(error);
        }

        public void WorkerExecuted(ITriggeredWork triggeredWork)
        {
            observables.TriggeredWorkExecuted(triggeredWork);
        }
    }
}