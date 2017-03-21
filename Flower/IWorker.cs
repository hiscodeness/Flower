namespace Flower
{
    using System.Threading.Tasks;

    public interface IWorker
    {
        Task Execute();
    }

    public interface IWorker<in TInput>
    {
        Task Execute(TInput input);
    }

    public interface IWorker<in TInput, TOutput>
    {
        Task<TOutput> Execute(TInput input);
    }
}
