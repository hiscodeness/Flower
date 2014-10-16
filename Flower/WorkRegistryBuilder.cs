using System;
using Flower.Works;

namespace Flower
{
    public class WorkRegistryBuilder
    {
        private readonly IWorkRegistry _workRegistry = new WorkRegistry(false);
        private bool _isBuilt;

        public IWorkRegistry Build()
        {
            if(_isBuilt)
            {
                throw new InvalidOperationException("Can build only once.");
            }

            _isBuilt = true;

            _workRegistry.ActivateAllWorks();
            return _workRegistry;
        }

        public IWork<TInput, TOutput> Register<TInput, TOutput>(
            IObservable<TInput> trigger, IWorker<TInput, TOutput> worker)
        {
            return _workRegistry.Register(trigger, worker);
        }
    }
}