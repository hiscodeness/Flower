namespace Flower
{
    using System.Threading.Tasks;

    public interface IWorker
    {
        void Execute();
    }

    public interface IWorker<in TInput>
    {
        void Execute(TInput input);
    }

    public interface IWorker<in TInput, TOutput>
    {
        TOutput Execute(TInput input);
    }
}
