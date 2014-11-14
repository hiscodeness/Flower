using Flower.WorkRunners;

namespace Flower.Works
{
    /// <summary>
    /// State of a work that has been triggered.
    /// </summary>
    public enum ExecutableWorkState
    {
        /// <summary>
        /// The work has been triggered and will be (or has been) submitted on a <see cref="IWorkRunner" />.
        /// </summary>
        Pending,

        /// <summary>
        /// The is being executed by a <see cref="IWorkRunner" />.
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