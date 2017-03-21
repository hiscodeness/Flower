using System;
using System.Collections.Generic;
using System.Linq;
using Flower.Works;

namespace Flower.WorkRunners
{
    using System.Threading.Tasks;

    /// <summary>
    /// A work runner that executes a single work immediately after being submitted.
    /// </summary>
    public sealed class ImmediateWorkRunner : IWorkRunner
    {
        private IExecutableWork executingWork;
        private bool isExecuted;

        public IEnumerable<IExecutableWork> PendingWorks { get { yield break; } }

        public IEnumerable<IExecutableWork> ExecutingWorks
        {
            get
            {
                return !isExecuted && executingWork != null
                           ? new[] { executingWork }
                           : Enumerable.Empty<IExecutableWork>();
            }
        }

        public async Task Submit(IExecutableWork executableWork)
        {
            if (executingWork != null)
            {
                throw new InvalidOperationException("Only one work can be submitted.");
            }

            executingWork = executableWork;
            await executingWork.Execute();
            isExecuted = true;
        }
    }
}