using System;
using Flower.WorkRunners;

namespace Flower
{
    public enum RegisterWorkBehavior
    {
        RegisterSuspended,
        RegisterActivated
    }

    public enum TriggerErrorBehavior
    {
        CompleteWorkAndThrow,
        SwallowErrorAndCompleteWork
    }

    public enum WorkerErrorBehavior
    {
        CompleteWorkAndThrow,
        SwallowErrorAndCompleteWork,
        RaiseExecutedAndContinue
    }

    public class WorkRegistryOptions
    {
        public static readonly WorkRegistryOptions Default = new WorkRegistryOptions();

        public WorkRegistryOptions(
            RegisterWorkBehavior registerWorkBehavior = RegisterWorkBehavior.RegisterSuspended,
            TriggerErrorBehavior triggerErrorBehavior = TriggerErrorBehavior.CompleteWorkAndThrow,
            IWorkRunnerResolver workRunnerResolver = null,
            WorkerErrorBehavior workerErrorBehavior = WorkerErrorBehavior.CompleteWorkAndThrow)
        {
            RegisterWorkBehavior = registerWorkBehavior;
            TriggerErrorBehavior = triggerErrorBehavior;
            WorkRunnerResolver = workRunnerResolver ?? new DefaultWorkRunnerResolver();
            WorkerErrorBehavior = workerErrorBehavior;
        }

        public RegisterWorkBehavior RegisterWorkBehavior { get; private set; }
        public TriggerErrorBehavior TriggerErrorBehavior { get; private set; }
        public IWorkRunnerResolver WorkRunnerResolver { get; private set; }
        public WorkerErrorBehavior WorkerErrorBehavior { get; private set; }

        public WorkRegistryOptions With(RegisterWorkBehavior registerWorkBehavior)
        {
            return new WorkRegistryOptions(registerWorkBehavior,
                                           TriggerErrorBehavior,
                                           WorkRunnerResolver,
                                           WorkerErrorBehavior);
        }

        public WorkRegistryOptions With(TriggerErrorBehavior triggerErrorBehavior)
        {
            return new WorkRegistryOptions(RegisterWorkBehavior,
                                           triggerErrorBehavior,
                                           WorkRunnerResolver,
                                           WorkerErrorBehavior);
        }

        public WorkRegistryOptions With(IWorkRunnerResolver workRunnerResolver)
        {
            if (workRunnerResolver == null)
            {
                throw new ArgumentNullException("workRunnerResolver");
            }

            return new WorkRegistryOptions(RegisterWorkBehavior,
                                           TriggerErrorBehavior,
                                           workRunnerResolver,
                                           WorkerErrorBehavior);
        }

        public WorkRegistryOptions With(WorkerErrorBehavior workerErrorBehavior)
        {
            return new WorkRegistryOptions(RegisterWorkBehavior,
                                           TriggerErrorBehavior,
                                           WorkRunnerResolver,
                                           workerErrorBehavior);
        }
    }
}