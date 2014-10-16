namespace Flower
{
    /// <summary>
    /// State of a work item.
    /// </summary>
    public enum WorkState
    {
        /// <summary>
        /// The work is registered in a <see cref="IWorkRegistry" /> and will respond to triggering.
        /// </summary>
        Active,

        /// <summary>
        /// The work is registered in a <see cref="IWorkRegistry" /> but suspended, i.e. ignoring
        /// triggering.
        /// </summary>
        Suspended,

        /// <summary>
        /// The work has been unregistered from the <see cref="IWorkRegistry" /> because the trigger
        /// observable ended in error.
        /// </summary>
        TriggerError,

        /// <summary>
        /// The work has been unregistered from the <see cref="IWorkRegistry" /> because the worker
        /// errored.
        /// </summary>
        WorkerError,

        /// <summary>
        /// The work has been unregistered from the <see cref="WorkRegistry" /> because the trigger
        /// completed.
        /// </summary>
        Completed
    }
}