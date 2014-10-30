using System;
using System.Threading.Tasks;
using Flower.WorkRunners;

namespace Flower
{
    /// <summary>
    /// A work that can be registered to run when triggered.
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// Runs the work.
        /// </summary>
        /// <remarks>
        /// Since <see cref="IWorker" />s are run by a <see cref="IWorkRunner" /> on a particular
        /// desired thread, any long running tasks started by this method can (and should) block the
        /// current thread. To wait for long running tasks within this method, consider using
        /// <see cref="Task{TResult}.Result" /> or <see cref="Task.Wait()" />.
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