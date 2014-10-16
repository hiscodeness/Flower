using System;
using Flower.Works;

namespace Flower
{
    public class WorkRegistryBuilder
    {
        private readonly IWorkRegistry workRegistry;
        private bool isBuilt;

        public static WorkRegistryBuilder Create()
        {
            return new WorkRegistryBuilder(new WorkRegistry(false));
        }

        private WorkRegistryBuilder(IWorkRegistry workRegistry)
        {
            this.workRegistry = workRegistry;
        }

        public IWorkRegistry Build()
        {
            if(isBuilt)
            {
                throw new InvalidOperationException("Can build only once.");
            }

            isBuilt = true;

            workRegistry.ActivateAllWorks();
            return workRegistry;
        }

        public IWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger, IWorker<TInput, TOutput> worker)
        {
            return workRegistry.Register(trigger, worker);
        }
    }
}