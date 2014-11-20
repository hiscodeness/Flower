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

    public class RegisterOptions
    {
        public static readonly RegisterOptions Default = new RegisterOptions();

        public RegisterOptions(TriggerErrorBehavior triggerErrorBehavior)
            : this(Default.RegisterWorkBehavior, triggerErrorBehavior) {}

        public RegisterOptions(IWorkRunnerResolver workRunnerResolver)
            : this(Default.RegisterWorkBehavior, Default.TriggerErrorBehavior, workRunnerResolver) {}

        public RegisterOptions(WorkerErrorBehavior workerErrorBehavior)
            : this(
                Default.RegisterWorkBehavior,
                Default.TriggerErrorBehavior,
                Default.WorkRunnerResolver,
                workerErrorBehavior) {}

        public RegisterOptions(RegisterOptions prototype)
            : this(
                prototype.RegisterWorkBehavior,
                prototype.TriggerErrorBehavior,
                prototype.WorkRunnerResolver,
                prototype.WorkerErrorBehavior) {}

        public RegisterOptions(
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

        public RegisterOptions With(RegisterWorkBehavior registerWorkBehavior)
        {
            return new RegisterOptions(registerWorkBehavior,
                                           TriggerErrorBehavior,
                                           WorkRunnerResolver,
                                           WorkerErrorBehavior);
        }

        public RegisterOptions With(TriggerErrorBehavior triggerErrorBehavior)
        {
            return new RegisterOptions(RegisterWorkBehavior,
                                           triggerErrorBehavior,
                                           WorkRunnerResolver,
                                           WorkerErrorBehavior);
        }

        public RegisterOptions With(IWorkRunnerResolver workRunnerResolver)
        {
            if (workRunnerResolver == null)
            {
                throw new ArgumentNullException("workRunnerResolver");
            }

            return new RegisterOptions(RegisterWorkBehavior,
                                           TriggerErrorBehavior,
                                           workRunnerResolver,
                                           WorkerErrorBehavior);
        }

        public RegisterOptions With(WorkerErrorBehavior workerErrorBehavior)
        {
            return new RegisterOptions(RegisterWorkBehavior,
                                           TriggerErrorBehavior,
                                           WorkRunnerResolver,
                                           workerErrorBehavior);
        }
    }
}