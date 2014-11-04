using System;

namespace Flower.Works
{
    internal class TriggerEvents : ITriggerEvents
    {
        public event Action<Exception> TriggerErrored;
        public event Action TriggerCompleted;

        public void RaiseTriggerErrored(Exception exception)
        {
            var handler = TriggerErrored;
            if (handler != null)
            {
                handler(exception);
            }
        }

        public void RaiseTriggerCompleted()
        {
            var handler = TriggerCompleted;
            if (handler != null)
            {
                handler();
            }
        }
    }
}