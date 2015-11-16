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
        CompleteWork
    }

    public enum WorkerErrorBehavior
    {
        CompleteWorkAndThrow,
        CompleteWork,
        Continue,
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
                prototype.WorkerErrorBehavior,
                prototype.WorkDecoratorFactory) {}
        
        public RegisterOptions(
            RegisterWorkBehavior registerWorkBehavior = RegisterWorkBehavior.RegisterActivated,
            TriggerErrorBehavior triggerErrorBehavior = TriggerErrorBehavior.CompleteWorkAndThrow,
            Func<IWork, IWorkRunner> workRunnerFactory = null,
            WorkerErrorBehavior workerErrorBehavior = WorkerErrorBehavior.CompleteWorkAndThrow,
            IWorkDecoratorFactory workDecoratorFactory = null)
        {
            RegisterWorkBehavior = registerWorkBehavior;
            TriggerErrorBehavior = triggerErrorBehavior;
            WorkRunnerFactory = workRunnerFactory ?? (_ => new ImmediateWorkRunner());
            WorkerErrorBehavior = workerErrorBehavior;
            WorkDecoratorFactory = workDecoratorFactory;
        }

        public RegisterWorkBehavior RegisterWorkBehavior { get; }
        public TriggerErrorBehavior TriggerErrorBehavior { get; }
        public Func<IWork, IWorkRunner> WorkRunnerFactory { get; }
        public WorkerErrorBehavior WorkerErrorBehavior { get; }
        public IWorkDecoratorFactory WorkDecoratorFactory { get; }
        public bool IsWorkDecorated => WorkDecoratorFactory != null;

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
                throw new ArgumentNullException(nameof(workRunnerFactory));
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
                throw new ArgumentNullException(nameof(workRunner));
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