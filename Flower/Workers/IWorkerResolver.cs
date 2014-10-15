namespace Flower.Workers
{
    public interface IWorkerResolver
    {
        IWorker Resolve();
        void Release(IWorker worker);
    }

    public interface IWorkerResolver<TInput>
    {
        IWorker<TInput> Resolve(TInput input);
        void Release(IWorker<TInput> input);
    }

    public interface IWorkerResolver<TInput, TOutput>
    {
        IWorker<TInput, TOutput> Resolve(TInput input);
        void Release(IWorker<TInput, TOutput> worker);
    }
}
