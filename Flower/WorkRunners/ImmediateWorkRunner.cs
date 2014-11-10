using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Flower.Works;

namespace Flower.WorkRunners
{
    /// <summary>
    /// A work runner that executes the work immediately after being submitted.
    /// </summary>
    public sealed class ImmediateWorkRunner : IWorkRunner, IDisposable
    {
        private readonly BlockingCollection<IExecutableWork> runningWorks =
            new BlockingCollection<IExecutableWork>();

        public IEnumerable<IExecutableWork> PendingWorks
        {
            get { yield break; }
        }

        public IEnumerable<IExecutableWork> ExecutingWorks
        {
            get { return runningWorks; }
        }

        public void Dispose()
        {
            runningWorks.Dispose();
        }

        public void Submit(IExecutableWork executableWork)
        {
            runningWorks.Add(executableWork);
            executableWork.Execute();
            runningWorks.TryTake(out executableWork);
        }
    }
}