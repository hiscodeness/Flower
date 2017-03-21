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
            Task.Factory.StartNew(DequeuePendingWorksThread, TaskCreationOptions.LongRunning).ConfigureAwait(false);
        }

        public int ThreadId { get; private set; }
        public IEnumerable<IExecutableWork> PendingWorks => pendingWorks;
        public IEnumerable<IExecutableWork> ExecutingWorks => executingWorks;

        public async Task Submit(IExecutableWork executableWork)
        {
            pendingWorks.Add(executableWork);
            await Task.CompletedTask.ConfigureAwait(false);
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
                Run(executableWork.Execute);
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

        /// <summary>
        /// Runs the specified asynchronous function synchronously on a single long-running thread.
        /// See http://blogs.msdn.com/b/pfxteam/archive/2012/01/20/10259049.aspx.
        /// </summary>
        /// <param name="func">The asynchronous function to execute.</param>
        private static void Run(Func<Task> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            var previousContext = SynchronizationContext.Current;
            try
            {
                // Establish the new context
                var threadContext = new SingleThreadSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(threadContext);

                // Invoke the function and alert the context to when it completes
                var task = func();
                if (task == null) throw new InvalidOperationException("No task provided.");
                task.ContinueWith(delegate { threadContext.Complete(); }, TaskScheduler.Default);

                // Pump continuations and propagate any exceptions
                threadContext.RunOnCurrentThread();
                task.GetAwaiter().GetResult();
            }
            finally { SynchronizationContext.SetSynchronizationContext(previousContext); }
        }

        /// <summary>
        /// Provides a SynchronizationContext that's single-threaded.
        /// </summary>
        private sealed class SingleThreadSynchronizationContext : SynchronizationContext
        {
            /// <summary>
            /// The queue of work items.
            /// </summary>
            private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> queue =
                new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

            /// <summary>
            /// Dispatches an asynchronous message to the synchronization context.
            /// </summary>
            /// <param name="callback">The System.Threading.SendOrPostCallback delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Post(SendOrPostCallback callback, object state)
            {
                if (callback == null)
                {
                    throw new ArgumentNullException(nameof(callback));
                }

                queue.Add(new KeyValuePair<SendOrPostCallback, object>(callback, state));
            }

            /// <summary>Not supported.</summary>
            public override void Send(SendOrPostCallback callback, object state)
            {
                throw new NotSupportedException("Synchronously sending is not supported.");
            }

            /// <summary>
            /// Runs an loop to process all queued work items.
            /// </summary>
            public void RunOnCurrentThread()
            {
                foreach (var workItem in queue.GetConsumingEnumerable())
                {
                    workItem.Key(workItem.Value);
                }
            }

            /// <summary>
            /// Notifies the context that no more work will arrive.
            /// </summary>
            public void Complete()
            {
                queue.CompleteAdding();
            }
        }
    }
}
