using System;
using System.Reactive.Linq;
using Flower.WorkRunners;

namespace Flower.Works
{
    internal class Work<TInput, TOutput> : WorkBase<TInput>, IWork<TInput, TOutput>
    {
        public Work(IWorkRegistration<TInput, TOutput> registration) : base(registration)
        {
            Registration = registration;
            Observables = new WorkObservables<IWork<TInput, TOutput>, ITriggeredWork<TInput, TOutput>>(this);
            Output = Executed.Select(executedWork => executedWork.Output);
        }

        new public IWorkRegistration<TInput, TOutput> Registration { get; private set; }
        public IObservable<ITriggeredWork<TInput, TOutput>> Triggered { get { return Observables.Triggered; } }
        public IObservable<ITriggeredWork<TInput, TOutput>> Executed { get { return Observables.Executed; } }
        public IObservable<TOutput> Output { get; private set; }
        internal WorkObservables<IWork<TInput, TOutput>, ITriggeredWork<TInput, TOutput>> Observables { get; private set; }

        protected override void TriggeredWorkCreated(ITriggeredWorkBase triggeredWork)
        {
            Observables.TriggeredWorkCreated(triggeredWork as ITriggeredWork<TInput, TOutput>);
        }

        protected override void TriggerErrored(Exception exception)
        {
            Observables.OnTriggerErrored(exception);
        }

        protected override void TriggerCompleted()
        {
            Observables.OnWorkCompleted();
        }

        protected override ITriggeredWorkBase CreateTriggeredWork(IWorkRunner workRunner, TInput input)
        {
            return new TriggeredWork<TInput, TOutput>(workRunner, this, input);
        }
    }
}