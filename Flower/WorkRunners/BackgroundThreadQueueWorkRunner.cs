namespace Flower.WorkRunners
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Flower.Works;

    public sealed class BackgroundThreadQueueWorkRunner : IWorkRunner, IDisposable
    {
        private readonly ManualResetEventSlim manualResetEvent = new ManualResetEventSlim();
        private readonly BlockingCollection<IExecutableWork> pendingWorks = new BlockingCollection<IExecutableWork>();
        private readonly BlockingCollection<IExecutableWork> executingWorks = new BlockingCollection<IExecutableWork>();

        public BackgroundThreadQueueWorkRunner()
        {
            Task.Factory.StartNew(DequeuePendingWorksThread, TaskCreationOptions.LongRunning);
        }

        public int ThreadId { get; private set; }
        public IEnumerable<IExecutableWork> PendingWorks => pendingWorks;
        public IEnumerable<IExecutableWork> ExecutingWorks => executingWorks;

        public void Submit(IExecutableWork executableWork)
        {
            pendingWorks.Add(executableWork);
        }

        private void DequeuePendingWorksThread()
        {
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            DequeueUntilAddingCompleted();
            manualResetEvent.Set();
        }

        private void DequeueUntilAddingCompleted()
        {
            foreach (var executableWork in pendingWorks.GetConsumingEnumerable().TakeWhile(ContinueDequeuing))
            {
                Debug.Assert(
                    executingWorks.Count == 0,
                    "Internal error, currently executing work count should be zero (0).");
                executingWorks.Add(executableWork);
                executableWork.Execute();
                Debug.Assert(
                    executingWorks.Count == 1,
                    "Internal error, currently executing work count should be exactly one (1).");
                executingWorks.Take();
            }
        }

        private bool ContinueDequeuing(IExecutableWork work)
        {
            return !pendingWorks.IsAddingCompleted;
        }

        public void Dispose()
        {
            if (pendingWorks.IsAddingCompleted)
            {
                return;
            }

            pendingWorks.CompleteAdding();
            manualResetEvent.Wait();
            pendingWorks.Dispose();
            manualResetEvent.Dispose();
        }        
    }
}
