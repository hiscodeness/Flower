using System.Collections.Concurrent;
using System.Collections.Generic;
using Flower.Works;

namespace Flower.WorkRunners
{
    /// <summary>
    ///     A work runner that runs the work immediately after triggering when it is appended.
    /// </summary>
    public class ImmediateWorkRunner : IWorkRunner
    {
        private readonly BlockingCollection<ITriggeredWorkBase> _runningWorks =
            new BlockingCollection<ITriggeredWorkBase>();

        /// <summary>
        ///     Gets the works still pending with the work runner.
        /// </summary>
        public IEnumerable<ITriggeredWorkBase> PendingWorks
        {
            get { yield break; }
        }

        /// <summary>
        ///     Gets the currently running active works.
        /// </summary>
        public IEnumerable<ITriggeredWorkBase> RunningWorks
        {
            get { return _runningWorks; }
        }

        public void Submit(ITriggeredWorkBase triggeredWork)
        {
            _runningWorks.Add(triggeredWork);
            triggeredWork.Execute();
            _runningWorks.TryTake(out triggeredWork);
        }
    }
}