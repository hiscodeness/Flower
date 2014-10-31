using System;
using Flower.Workers;

namespace Flower.Works
{
    internal abstract class WorkRegistrationBase<TInput>
    {
        protected WorkRegistrationBase(
            IWorkRegistry workRegistry, IObservable<TInput> trigger)
        {
            WorkRegistry = workRegistry;
            Trigger = trigger;
        }

        public IWorkRegistry WorkRegistry { get; private set; }
        public WorkRegistryOptions Options { get { return WorkRegistry.Options; } }
        public IObservable<TInput> Trigger { get; private set; }
    }
}