namespace Flower.Triggers
{
    using System;
    public interface ITriggerable<TInput> : IObservable<TInput>
    {
        void Trigger(TInput input);
    }
}
