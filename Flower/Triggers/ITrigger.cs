namespace Flower.Triggers
{
    using System;
    public interface ITrigger<TInput> : IObservable<TInput>
    {
        void Trigger(TInput input);
    }
}
