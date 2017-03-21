namespace Flower.Works
{
    using System;

    internal class TriggerEvents : ITriggerEvents
    {
        public event Action<Exception> TriggerErrored;
        public event Action TriggerCompleted;

        public void RaiseTriggerErrored(Exception exception)
        {
            var handler = TriggerErrored;
            handler?.Invoke(exception);
        }

        public void RaiseTriggerCompleted()
        {
            var handler = TriggerCompleted;
            handler?.Invoke();
        }
    }
}
