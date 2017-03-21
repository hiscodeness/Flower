namespace Flower.Works
{
    using System;

    internal interface ITriggerEvents
    {
        event Action<Exception> TriggerErrored;
        event Action TriggerCompleted;

        void RaiseTriggerErrored(Exception exception);
        void RaiseTriggerCompleted();
    }
}
