namespace Flower
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Flower.Works;
    using Flower.Workers;

    public interface IWorkRegistry
    {
        WorkOptions Options { get; }

        IEnumerable<IWork> Works { get; }

        IActionWork RegisterMethod<TInput>(
            IObservable<TInput> trigger,
            Func<Task> method,
            WorkOptions options = null);

        IActionWork<TInput> RegisterMethod<TInput>(
            IObservable<TInput> trigger,
            Func<TInput, Task> method,
            WorkOptions options = null);

        IFuncWork<TInput, TOutput> RegisterMethod<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<TInput, TOutput> method,
            WorkOptions options = null);

        IActionWork RegisterWorker<TInput>(
            IObservable<TInput> trigger,
            IWorker worker,
            WorkOptions options = null);
        
        IActionWork<TInput> RegisterWorker<TInput>(
            IObservable<TInput> trigger,
            IWorker<TInput> worker,
            WorkOptions options = null);

        IFuncWork<TInput, TOutput> RegisterWorker<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorker<TInput, TOutput> worker,
            WorkOptions options = null);

        IActionWork RegisterResolver<TInput>(
            IObservable<TInput> trigger,
            IWorkerResolver resolver,
            WorkOptions options = null);

        IActionWork<TInput> RegisterResolver<TInput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput> resolver,
            WorkOptions options = null);

        IFuncWork<TInput, TOutput> RegisterResolver<TInput, TOutput>(
            IObservable<TInput> trigger,
            IWorkerResolver<TInput, TOutput> resolver,
            WorkOptions options = null);


        IActionWork RegisterFactory<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker>> factory,
            WorkOptions options = null);

        IActionWork<TInput> RegisterFactory<TInput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput>>> factory,
            WorkOptions options = null);

        IFuncWork<TInput, TOutput> RegisterFactory<TInput, TOutput>(
            IObservable<TInput> trigger,
            Func<IScope<IWorker<TInput, TOutput>>> factory,
            WorkOptions options = null);

        void Complete(IWork work);
        void CompleteAll();
    }
}