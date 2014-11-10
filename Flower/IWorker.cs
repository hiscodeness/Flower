namespace Flower
{
    public interface IWorker
    {
        void Execute();
    }

    public interface IWorker<in TInput>
    {
        void Execute(TInput input);
    }

    public interface IWorker<in TInput, out TOutput>
    {
        TOutput Execute(TInput input);
    }
}