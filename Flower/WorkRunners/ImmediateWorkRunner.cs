using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Flower.Works;

namespace Flower.WorkRunners
{
    /// <summary>
    /// A work runner that runs the work immediately after triggering when it is appended.
    /// </summary>
    public sealed class ImmediateWorkRunner : IWorkRunner, IDisposable
    {
        private readonly BlockingCollection<ITriggeredWorkBase> runningWorks =
            new BlockingCollection<ITriggeredWorkBase>();

        /// <summary>
        /// Gets the works still pending with the work runner.
        /// </summary>
        public IEnumerable<ITriggeredWorkBase> PendingWorks
        {
            get { yield break; }
        }

        /// <summary>
        /// Gets the currently running active works.
        /// </summary>
        public IEnumerable<ITriggeredWorkBase> RunningWorks
        {
            get { return runningWorks; }
        }

        public void Dispose()
        {
            runningWorks.Dispose();
        }

        public void Submit(ITriggeredWorkBase triggeredWork)
        {
            runningWorks.Add(triggeredWork);
            triggeredWork.Execute();
            runningWorks.TryTake(out triggeredWork);
        }
    }
}