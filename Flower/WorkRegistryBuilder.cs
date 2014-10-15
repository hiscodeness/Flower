using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Flower.Works;

namespace Flower
{
    public class WorkRegistryBuilder
    {
        private bool _isBuilt;
        private readonly IWorkRegistry _workRegistry = new WorkRegistry(false);

        public IWork<TInput, TOutput> Register<TInput, TOutput>(IObservable<TInput> trigger, IWorker<TInput, TOutput> worker)
        {
            return _workRegistry.Register(trigger, worker);
        }

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
    }
}
