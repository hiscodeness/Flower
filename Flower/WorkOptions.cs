namespace Flower
{
    using System;
    using Flower.WorkRunners;
    using Flower.Works;

    public enum WorkRegisterMode
    {
        Activated,
        Suspended
    }

    public enum TriggerErrorMode
    {
        /// <summary>
        /// Trigger error appears on the works error stream.
        /// </summary>
        ErrorWork,

        /// <summary>
        /// Trigger error does not appear on the works error stream, instead the worker is only completed.
        /// </summary>
        CompleteWork
    }

    public enum WorkerErrorMode
    {
        Continue,
        CompleteWork,
        CompleteWorkAndThrow
    }

    public class WorkOptions
    {
        public static readonly WorkOptions Default = new WorkOptions();

        public WorkOptions(TriggerErrorMode triggerErrorMode)
            : this(Default.WorkRegisterMode, triggerErrorMode)
        {
        }

        public WorkOptions(Func<IWork, IWorkRunner> workRunnerFactory)
            : this(Default.WorkRegisterMode, Default.TriggerErrorMode, workRunnerFactory)
        {
        }

        public WorkOptions(IWorkRunner workRunner)
            : this(Default.WorkRegisterMode, Default.TriggerErrorMode, _ => workRunner)
        {
        }

        public WorkOptions(WorkerErrorMode workerErrorMode)
            : this(
                Default.WorkRegisterMode,
                Default.TriggerErrorMode,
                Default.WorkRunnerFactory,
                workerErrorMode)
        {
        }

        public WorkOptions(WorkOptions prototype)
            : this(
                prototype.WorkRegisterMode,
                prototype.TriggerErrorMode,
                prototype.WorkRunnerFactory,
                prototype.WorkerErrorMode,
                prototype.WorkDecoratorFactory)
        {
        }

        public WorkOptions(
            WorkRegisterMode workRegisterMode = WorkRegisterMode.Activated,
            TriggerErrorMode triggerErrorMode = TriggerErrorMode.ErrorWork,
            Func<IWork, IWorkRunner> workRunnerFactory = null,
            WorkerErrorMode workerErrorMode = WorkerErrorMode.Continue,
            IWorkDecoratorFactory workDecoratorFactory = null)
        {
            WorkRegisterMode = workRegisterMode;
            TriggerErrorMode = triggerErrorMode;
            WorkRunnerFactory = workRunnerFactory ?? (_ => new ImmediateWorkRunner());
            WorkerErrorMode = workerErrorMode;
            WorkDecoratorFactory = workDecoratorFactory;
        }

        public WorkRegisterMode WorkRegisterMode { get; }
        public TriggerErrorMode TriggerErrorMode { get; }
        public Func<IWork, IWorkRunner> WorkRunnerFactory { get; }
        public WorkerErrorMode WorkerErrorMode { get; }
        public IWorkDecoratorFactory WorkDecoratorFactory { get; }
        public bool IsWorkDecorated => WorkDecoratorFactory != null;

        public WorkOptions With(WorkRegisterMode workRegisterMode)
        {
            return new WorkOptions(
                workRegisterMode,
                TriggerErrorMode,
                WorkRunnerFactory,
                WorkerErrorMode);
        }

        public WorkOptions With(TriggerErrorMode triggerErrorMode)
        {
            return new WorkOptions(
                WorkRegisterMode,
                triggerErrorMode,
                WorkRunnerFactory,
                WorkerErrorMode);
        }

        public WorkOptions With(Func<IWork, IWorkRunner> workRunnerFactory)
        {
            if (workRunnerFactory == null)
            {
                throw new ArgumentNullException(nameof(workRunnerFactory));
            }

            return new WorkOptions(
                WorkRegisterMode,
                TriggerErrorMode,
                workRunnerFactory,
                WorkerErrorMode);
        }

        public WorkOptions With(IWorkRunner workRunner)
        {
            if (workRunner == null)
            {
                throw new ArgumentNullException(nameof(workRunner));
            }

            return new WorkOptions(
                WorkRegisterMode,
                TriggerErrorMode,
                _ => workRunner,
                WorkerErrorMode);
        }

        public WorkOptions With(WorkerErrorMode workerErrorMode)
        {
            return new WorkOptions(
                WorkRegisterMode,
                TriggerErrorMode,
                WorkRunnerFactory,
                workerErrorMode);
        }
    }
}
