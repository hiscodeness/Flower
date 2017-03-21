using System.Collections.Generic;
using Flower.Works;

namespace Flower.WorkRunners
{
    using System.Threading.Tasks;

    /// <summary>
    /// Executes triggered works.
    /// </summary>
    public interface IWorkRunner
    {
        IEnumerable<IExecutableWork> PendingWorks { get; }
        IEnumerable<IExecutableWork> ExecutingWorks { get; }

        /// <summary>
        /// Submit a work to be executed by this runner at the next opportunity.
        /// </summary>
        /// <param name="executableWork">The work to execute at the next opportunity.</param>
        void Submit(IExecutableWork executableWork);
    }
}
