namespace Flower.WorkRunners
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Flower.Works;

    /// <summary>
    /// An <see cref="IWorkRunner"/> that executes the submitted works on background threads from the thread pool.
    /// </summary>
    public class ThreadPoolWorkRunner : IWorkRunner
    {
        private readonly BlockingCollection<IExecutableWork> executingWorks = new BlockingCollection<IExecutableWork>();
        
        public async Task Submit(IExecutableWork executableWork)
        {
            executingWorks.Add(executableWork);
#pragma warning disable 4014 // Begin executing in the background thread, don't wait for it to complete
            Task.Run(executableWork.Execute).ContinueWith(
#pragma warning restore 4014
                _ =>
                {
                    executingWorks.TryTake(out executableWork);
                });
            await Task.CompletedTask;
        }

        public IEnumerable<IExecutableWork> PendingWorks { get; } = Enumerable.Empty<IExecutableWork>();

        public IEnumerable<IExecutableWork> ExecutingWorks => executingWorks;
    }
}
