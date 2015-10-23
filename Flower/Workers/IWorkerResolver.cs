namespace Flower.Workers
{
    public interface IWorkerResolver
    {
        IWorker Resolve();
        void Release(IWorker worker);
    }

    public interface IWorkerResolver<TInput>
    {
        IWorker<TInput> Resolve();
        void Release(IWorker<TInput> worker);
    }

    public interface IWorkerResolver<TInput, TOutput>
    {
        IWorker<TInput, TOutput> Resolve();
        void Release(IWorker<TInput, TOutput> worker);
    }
}
