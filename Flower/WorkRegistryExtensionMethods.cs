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
         IWorker worker)
        {
            return workRegistry.Register(trigger, WorkerResolver.CreateFromInstance(worker));
        }

        public static IActionWork<TInput> Register<TInput>(
           this IWorkRegistry workRegistry,
           IObservable<TInput> trigger,
           IWorker<TInput> worker)
        {
            return workRegistry.Register(trigger, WorkerResolver.CreateFromInstance(worker));
        }

        public static IFuncWork<TInput, TOutput> Register<TInput, TOutput>(
            this IWorkRegistry workRegistry,
            IObservable<TInput> trigger,
            IWorker<TInput, TOutput> worker)
        {
            return workRegistry.Register(trigger, WorkerResolver.CreateFromInstance(worker));
        }
    }
}