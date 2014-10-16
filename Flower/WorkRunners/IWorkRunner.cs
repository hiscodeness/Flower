using System.Collections.Generic;
using Flower.Works;

namespace Flower.WorkRunners
{
    /// <summary>
    /// Runs triggered work items.
    /// </summary>
    public interface IWorkRunner
    {
        /// <summary>
        /// Gets the triggered work items still pending with this <see cref="IWorkRunner" />.
        /// </summary>
        IEnumerable<ITriggeredWorkBase> PendingWorks { get; }

        /// <summary>
        /// Gets the currently running active work items.
        /// </summary>
        IEnumerable<ITriggeredWorkBase> RunningWorks { get; }

        /// <summary>
        /// Submit a work item to be run by this runner at the next opportunity.
        /// </summary>
        /// <param name="triggeredWork">The work item to run at the next opportunity.</param>
        void Submit(ITriggeredWorkBase triggeredWork);
    }
}