using System;
using System.Threading.Tasks;
using Flower.WorkRunners;
using Flower.Works;

namespace Flower.Tests.TestDoubles
{
    internal class TestExecutableWork : IExecutableFuncWork<int, int>
    {
        private readonly TestWorkIntToIntSquared work = new TestWorkIntToIntSquared();
        private readonly TestWorkerIntToIntSquared worker = new TestWorkerIntToIntSquared();
        private readonly TimeSpan executeDelay;
        private readonly Action<TestExecutableWork> beforeExecute;
        private readonly Action<TestExecutableWork> afterExecute;

        public TestExecutableWork(IWorkRunner workRunner, TimeSpan executeDelay, Action<TestExecutableWork> beforeExecute = null, Action<TestExecutableWork> afterExecute = null, int input = 0)
        {
            WorkRunner = workRunner;
            this.executeDelay = executeDelay;
            this.beforeExecute = beforeExecute ?? (_ => {});
            this.afterExecute = afterExecute ?? (_ => {});
            Input = input;
        }

        public int Input { get; private set; }
        IFuncWork<int, int> ITriggeredFuncWork<int, int>.Work { get { return work; } }
        IWork<int> ITriggeredWork<int>.Work { get { return work; } }
        IWork ITriggeredWork.Work { get { return work; } }
        public IWorkRunner WorkRunner { get; private set; }
        public ExecutableWorkState State { get; private set; }
        public void Execute()
        {
            beforeExecute(this);
            
            try
            {
                ExecuteWithCurrentInput();
            }
            catch (Exception error)
            {
                Error = error;
            }

            afterExecute(this);
        }

        private void ExecuteWithCurrentInput()
        {
            if (!executeDelay.Equals(default(TimeSpan)))
            {
                Task.Delay(executeDelay).Wait();
            }
            Output = worker.Execute(Input);
        }

        public Exception Error { get; private set; }
        public IWorker<int, int> Worker { get { return worker; } }
        public int Output { get; private set; }
    }
}
