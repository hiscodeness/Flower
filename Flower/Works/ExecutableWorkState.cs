using Flower.WorkRunners;

namespace Flower.Works
{
    /// <summary>
    /// State of a work that has been triggered.
    /// </summary>
    public enum ExecutableWorkState
    {
        /// <summary>
        /// The work has been triggered and is being submitted on a <see cref="IWorkRunner" />,
        /// which will execute the work at the next opportunity.
        /// </summary>
        Pending,

        /// <summary>
        /// The work is being executed by a <see cref="IWorkRunner" />.
        /// </summary>
        Executing,

        /// <summary>
        /// The work was executed and finished succesfully.
        /// </summary>
        Success,

        /// <summary>
        /// The work was executed and errored (threw an exception).
        /// </summary>
        Error
    }
}