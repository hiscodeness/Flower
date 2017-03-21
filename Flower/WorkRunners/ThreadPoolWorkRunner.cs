namespace Flower.WorkRunners
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Flower.Works;

    /// <summary>
    /// An <see cref="IWorkRunner" /> that executes the submitted works on background threads from the thread pool.
    /// </summary>
    public class ThreadPoolWorkRunner : IWorkRunner
    {
        private readonly BlockingCollection<IExecutableWork> executingWorks = new BlockingCollection<IExecutableWork>();

        public async Task Submit(IExecutableWork executableWork)
        {
            executingWorks.Add(executableWork);
            var finished = Task.Run(executableWork.Execute).ContinueWith(
                _ =>
                {
                    // Work executed on a background thread has finished
                    executingWorks.TryTake(out executableWork);
                });
            await Task.CompletedTask;
        }

        public IEnumerable<IExecutableWork> PendingWorks { get; } = Enumerable.Empty<IExecutableWork>();

        public IEnumerable<IExecutableWork> ExecutingWorks => executingWorks;
    }
}
