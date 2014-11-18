using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flower.Works;

namespace Flower.WorkRunners
{
    public class BackgroundThreadQueueWorkRunner : IWorkRunner
    {
        private readonly BlockingCollection<IExecutableWork> pendingWorks = new BlockingCollection<IExecutableWork>();
        private readonly BlockingCollection<IExecutableWork> executingWorks = new BlockingCollection<IExecutableWork>();

        public BackgroundThreadQueueWorkRunner()
        {
            // All tasks are queued and dequeued on a separate long running thread
            Task.Factory.StartNew(DequeuePendingWorksThread, TaskCreationOptions.LongRunning);
        }

        public IEnumerable<IExecutableWork> PendingWorks { get { return pendingWorks; } }
        public IEnumerable<IExecutableWork> ExecutingWorks { get { return executingWorks; } }
        
        public void Submit(IExecutableWork executableWork)
        {
            pendingWorks.Add(executableWork);
        }

        private void DequeuePendingWorksThread()
        {
            foreach (var work in pendingWorks.GetConsumingEnumerable())
            {
                executingWorks.Add(work);
                work.Execute();
                executingWorks.Take();
            }

            pendingWorks.Dispose();
        }
    }
}
