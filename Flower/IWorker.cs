using Flower.WorkRunners;

namespace Flower
{
    /// <summary>
    /// A worker that can be registered to execute on a <see cref="IWorkRunner"/> after having been triggered.
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// Executes the work.
        /// </summary>
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