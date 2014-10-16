using System;
using System.Threading.Tasks;
using Flower.WorkRunners;

namespace Flower
{
    /// <summary>
    /// A work that can be planned to be run based on a trigger.
    /// </summary>
    /// <remarks>
    /// If the <see cref="IWorker" /> implements <see cref="IDisposable" />,
    /// it will be disposed after it has been run.
    /// </remarks>
    public interface IWorker
    {
        /// <summary>
        /// Runs the work.
        /// </summary>
        /// <remarks>
        /// Since <see cref="IWorker" />s are already planned to run with a
        ///     <see cref="IWorkRunner" /> on a particular desired thread, any long running tasks
        /// started by this method can (and should) block the current thread. To wait for long
        /// running tasks within this method, consider using <see cref="Task{TResult}.Result" /> or
        ///     <see cref="Task.Wait()" />.
        /// </remarks>
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