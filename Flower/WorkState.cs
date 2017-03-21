namespace Flower
{
    /// <summary>
    /// State of a work item.
    /// </summary>
    public enum WorkState
    {
        /// <summary>
        /// The work is registered in a <see cref="IWorkRegistry" /> but suspended, i.e. ignoring
        /// triggering.
        /// </summary>
        Suspended,

        /// <summary>
        /// The work is registered in a <see cref="IWorkRegistry" /> and will respond to triggering.
        /// </summary>
        Active,

        /// <summary>
        /// The work has been removed from the <see cref="IWorkRegistry" /> because the trigger
        /// observable ended in error.
        /// </summary>
        TriggerError,

        /// <summary>
        /// The work has been removed from the <see cref="IWorkRegistry" /> because the worker
        /// errored.
        /// </summary>
        WorkerError,

        /// <summary>
        /// The work has been completed and removed from the <see cref="IWorkRegistry" />.
        /// </summary>
        /// <remarks>
        /// Works are completed when the trigger completes or errors. Workers can also be completed
        /// manually by calling <see cref="IWorkRegistry.Complete" />.
        /// </remarks>
        Completed
    }
}
