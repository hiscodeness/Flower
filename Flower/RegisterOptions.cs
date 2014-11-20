using System;
using Flower.WorkRunners;
using Flower.Works;

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

        public RegisterOptions(Func<IWork, IWorkRunner> workRunnerFactory)
            : this(Default.RegisterWorkBehavior, Default.TriggerErrorBehavior, workRunnerFactory) {}

        public RegisterOptions(IWorkRunner workRunner)
            : this(Default.RegisterWorkBehavior, Default.TriggerErrorBehavior, _ => workRunner) { }

        public RegisterOptions(WorkerErrorBehavior workerErrorBehavior)
            : this(
                Default.RegisterWorkBehavior,
                Default.TriggerErrorBehavior,
                Default.WorkRunnerFactory,
                workerErrorBehavior) {}

        public RegisterOptions(RegisterOptions prototype)
            : this(
                prototype.RegisterWorkBehavior,
                prototype.TriggerErrorBehavior,
                prototype.WorkRunnerFactory,
                prototype.WorkerErrorBehavior) {}
        
        public RegisterOptions(
            RegisterWorkBehavior registerWorkBehavior = RegisterWorkBehavior.RegisterActivated,
            TriggerErrorBehavior triggerErrorBehavior = TriggerErrorBehavior.CompleteWorkAndThrow,
            Func<IWork, IWorkRunner> workRunnerFactory = null,
            WorkerErrorBehavior workerErrorBehavior = WorkerErrorBehavior.CompleteWorkAndThrow)
        {
            RegisterWorkBehavior = registerWorkBehavior;
            TriggerErrorBehavior = triggerErrorBehavior;
            WorkRunnerFactory = workRunnerFactory ?? (_ => new ImmediateWorkRunner());
            WorkerErrorBehavior = workerErrorBehavior;
        }

        public RegisterWorkBehavior RegisterWorkBehavior { get; private set; }
        public TriggerErrorBehavior TriggerErrorBehavior { get; private set; }
        public Func<IWork, IWorkRunner> WorkRunnerFactory { get; private set; }
        public WorkerErrorBehavior WorkerErrorBehavior { get; private set; }

        public RegisterOptions With(RegisterWorkBehavior registerWorkBehavior)
        {
            return new RegisterOptions(
                registerWorkBehavior,
                TriggerErrorBehavior,
                WorkRunnerFactory,
                WorkerErrorBehavior);
        }

        public RegisterOptions With(TriggerErrorBehavior triggerErrorBehavior)
        {
            return new RegisterOptions(
                RegisterWorkBehavior,
                triggerErrorBehavior,
                WorkRunnerFactory,
                WorkerErrorBehavior);
        }

        public RegisterOptions With(Func<IWork, IWorkRunner> workRunnerFactory)
        {
            if (workRunnerFactory == null)
            {
                throw new ArgumentNullException("workRunnerFactory");
            }

            return new RegisterOptions(
                RegisterWorkBehavior,
                TriggerErrorBehavior,
                workRunnerFactory,
                WorkerErrorBehavior);
        }
        
        public RegisterOptions With(IWorkRunner workRunner)
        {
            if (workRunner == null)
            {
                throw new ArgumentNullException("workRunner");
            }

            return new RegisterOptions(
                RegisterWorkBehavior,
                TriggerErrorBehavior,
                _ => workRunner,
                WorkerErrorBehavior);
        }

        public RegisterOptions With(WorkerErrorBehavior workerErrorBehavior)
        {
            return new RegisterOptions(
                RegisterWorkBehavior,
                TriggerErrorBehavior,
                WorkRunnerFactory,
                workerErrorBehavior);
        }
    }
}