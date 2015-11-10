namespace Flower
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Flower.Works;
    using Flower.Workers;

    public interface IWorkRegistry
    {
        RegisterOptions DefaultOptions { get; }

        IEnumerable<IWork> Works { get; }

        IActionWork RegisterMethod<TInput>(
            IObservable<TInput> trigger,
            Func<Task> method,
            RegisterOptions options = null);

        IActionWork<TInput> RegisterMethod<TInput>(
            IObservable<TInput> trigger,
            Func<TInput, Task> method,
            RegisterOptions options = null);

        IFuncWork<TInput, TOutput> RegisterMethod<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<TInput, Task<TOutput>> method,
            RegisterOptions options = null);

        IActionWork RegisterWorker<TInput>(
            IObservable<TInput> trigger,
            IWorker worker,
            RegisterOptions options = null);
        
        IActionWork<TInput> RegisterWorker<TInput>(
            IObservable<TInput> trigger,
            IWorker<TInput> worker,
            RegisterOptions options = null);

        IFuncWork<TInput, TOutput> RegisterWorker<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorker<TInput, TOutput> worker,
            RegisterOptions options = null);

        IActionWork RegisterResolver<TInput>(
            IObservable<TInput> trigger,
            IWorkerResolver resolver,
            RegisterOptions options = null);

        IActionWork<TInput> RegisterResolver<TInput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput> resolver,
            RegisterOptions options = null);

        IFuncWork<TInput, TOutput> RegisterResolver<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> resolver,
            RegisterOptions options = null);


        IActionWork RegisterFactory<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker>> factory,
            RegisterOptions options = null);

        IActionWork<TInput> RegisterFactory<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput>>> factory,
            RegisterOptions options = null);

        IFuncWork<TInput, TOutput> RegisterFactory<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput, TOutput>>> factory,
            RegisterOptions options = null);

        void Complete(IWork work);
        void CompleteAll();
    }
}