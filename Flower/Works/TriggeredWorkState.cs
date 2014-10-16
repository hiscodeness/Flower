using Flower.WorkRunners;

namespace Flower.Works
{
    /// <summary>
    /// State of a work that has been triggered.
    /// </summary>
    public enum TriggeredWorkState
    {
        /// <summary>
        /// The triggered work has been created but not yet submitted to be run on a <see cref="IWorkRunner" />.
        /// </summary>
        Created,

        /// <summary>
        /// The work has been triggered and submitted to be run on a <see cref="IWorkRunner" />.
        /// </summary>
        Submitted,

        /// <summary>
        /// The work has been started by a <see cref="IWorkRunner" /> and it is currently running.
        /// </summary>
        Executing,

        /// <summary>
        /// The work was run and finished succesfully.
        /// </summary>
        Success,

        /// <summary>
        /// The work was run and failed (threw an exception).
        /// </summary>
        Failure
    }
}