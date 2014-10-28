using Flower.WorkRunners;

namespace Flower
{
    public enum WorkActivationBehavior
    {
        RegisterSuspended,
        RegisterActivated
    }

    public enum WorkerErrorBehavior
    {
        Throw,
        Ignore,
        Complete
    }

    public class WorkRegistryOptions
    {
        public static readonly WorkRegistryOptions Default = new WorkRegistryOptions();

        public WorkRegistryOptions(
            WorkActivationBehavior workActivationBehavior = WorkActivationBehavior.RegisterSuspended,
            IWorkRunnerResolver workRunnerResolver = null,
            WorkerErrorBehavior workerErrorBehavior = WorkerErrorBehavior.Throw)
        {
            WorkActivationBehavior = workActivationBehavior;
            WorkRunnerResolver = workRunnerResolver ?? new DefaultWorkRunnerResolver();
            WorkerErrorBehavior = workerErrorBehavior;
        }

        public WorkActivationBehavior WorkActivationBehavior { get; private set; }
        public IWorkRunnerResolver WorkRunnerResolver { get; private set; }
        public WorkerErrorBehavior WorkerErrorBehavior { get; private set; }

        public WorkRegistryOptions With(WorkActivationBehavior workActivationBehavior)
        {
            return new WorkRegistryOptions(workActivationBehavior,
                                           WorkRunnerResolver,
                                           WorkerErrorBehavior);
        }

        public WorkRegistryOptions With(IWorkRunnerResolver workRunnerResolver)
        {
            return new WorkRegistryOptions(WorkActivationBehavior,
                                           workRunnerResolver,
                                           WorkerErrorBehavior);
        }

        public WorkRegistryOptions With(WorkerErrorBehavior workerErrorBehavior)
        {
            return new WorkRegistryOptions(WorkActivationBehavior,
                                           WorkRunnerResolver,
                                           workerErrorBehavior);
        }
    }
}