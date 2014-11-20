﻿using System;
using Flower.Workers;
using Flower.Works;

namespace Flower
{
    public static class WorkRegistryExtensionMethods
    {
        public static IActionWork Register<TInput>(
            this IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            IWorker worker,
            WorkRegistryOptions options = null)
        {
            return workRegistry.Register(trigger, WorkerScope.Instance(worker), options);
        }

        public static IActionWork<TInput> Register<TInput>(
            this IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            IWorker<TInput> worker,
            WorkRegistryOptions options = null)
        {
            return workRegistry.Register(trigger, WorkerScope.Instance(worker), options);
        }

        public static IFuncWork<TInput, TOutput> Register<TInput, TOutput>(
            this IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            IWorker<TInput, TOutput> worker,
            WorkRegistryOptions options = null)
        {
            return workRegistry.Register(trigger, WorkerScope.Instance(worker), options);
        }
    }
}