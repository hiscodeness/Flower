using System;
using Flower.WorkRunners;

namespace Flower
{
    public enum RegisterWorkBehavior
    {
        RegisterActivated,
        RegisterSuspended
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
        RaiseExecutedAndCompleteWork,
        RaiseExecutedAndContinue,
        SwallowErrorAndContinue
    }

    public class WorkRegistryOptions
    {
        public static readonly WorkRegistryOptions Default = new WorkRegistryOptions();

        public WorkRegistryOptions(TriggerErrorBehavior triggerErrorBehavior)
            : this(Default.RegisterWorkBehavior, triggerErrorBehavior) {}

        public WorkRegistryOptions(IWorkRunnerResolver workRunnerResolver)
            : this(Default.RegisterWorkBehavior, Default.TriggerErrorBehavior, workRunnerResolver) {}

        public WorkRegistryOptions(WorkerErrorBehavior workerErrorBehavior)
            : this(
                Default.RegisterWorkBehavior,
                Default.TriggerErrorBehavior,
                Default.WorkRunnerResolver,
                workerErrorBehavior) {}

        public WorkRegistryOptions(
            RegisterWorkBehavior registerWorkBehavior = RegisterWorkBehavior.RegisterActivated,
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