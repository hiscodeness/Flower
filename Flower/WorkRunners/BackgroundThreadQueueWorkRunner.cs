using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flower.Works;

namespace Flower.WorkRunners
{
    public sealed class BackgroundThreadQueueWorkRunner : IWorkRunner, IDisposable
    {
        private readonly ManualResetEventSlim manualResetEvent = new ManualResetEventSlim();
        private readonly BlockingCollection<IExecutableWork> pendingWorks = new BlockingCollection<IExecutableWork>();
        private IExecutableWork executingWork;

        public BackgroundThreadQueueWorkRunner()
        {
            Task.Factory.StartNew(DequeuePendingWorksThread, TaskCreationOptions.LongRunning);
        }

        public IEnumerable<IExecutableWork> PendingWorks => pendingWorks;
        public IEnumerable<IExecutableWork> ExecutingWorks => new[] { executingWork };

        public void Submit(IExecutableWork executableWork)
        {
            pendingWorks.Add(executableWork);
        }

        private void DequeuePendingWorksThread()
        {
            DequeueUntilAddingCompleted();
            manualResetEvent.Set();
        }

        private void DequeueUntilAddingCompleted()
        {
            foreach (var work in pendingWorks.GetConsumingEnumerable().TakeWhile(ContinueDequeuing)) 
            {
                Debug.Assert(executingWork == null, "Internal error, currently executing work should have been null.");
                executingWork = work;
                work.Execute();
                executingWork = null;
            }
        }

        private bool ContinueDequeuing(IExecutableWork work)
        {
            return !pendingWorks.IsAddingCompleted;
        }

        public void Dispose()
        {
            if (pendingWorks.IsAddingCompleted) return;

            pendingWorks.CompleteAdding();
            manualResetEvent.Wait();
            pendingWorks.Dispose();
            manualResetEvent.Dispose();
        }
    }
}
