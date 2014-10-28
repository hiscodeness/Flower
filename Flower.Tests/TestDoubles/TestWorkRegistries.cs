using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flower.Tests.TestDoubles
{
    public static class WorkRegistryFactory
    {
        public static WorkRegistry CreateAutoActivating()
        {
            var options = new WorkRegistryOptions(RegisterWorkBehavior.RegisterActivated,
                                                  TriggerErrorBehavior.CompleteWork);
            return new WorkRegistry(options);
        }
    }
}
