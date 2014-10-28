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
            return
                new WorkRegistry(WorkRegistryOptions.Default.With(WorkActivationBehavior.RegisterActivated));
        }
    }
}
