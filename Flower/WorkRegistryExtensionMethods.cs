using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public static class WorkRegistryExtensionMethods
    {
        public static IWork<TInput, TOutput> Register<TInput, TOutput>(
            this IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            IWorker<TInput, TOutput> worker)
        {
            return workRegistry.Register(trigger, WorkerResolver.CreateFromInstance(worker));
        }
    }
}
