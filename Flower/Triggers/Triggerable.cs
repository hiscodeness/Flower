namespace Flower.Triggers
{
    using System;
    using System.Reactive.Linq;

    public class Triggerable<TInput> : ITriggerable<TInput>
    {
        private event Action<TInput> Triggered;
        private readonly IObservable<TInput> observable;

        public Triggerable()
        {
            observable = Observable.FromEvent<TInput>(
                e => Triggered += e,
                e => Triggered -= e);
        }

        public IDisposable Subscribe(IObserver<TInput> observer)
        {
            return observable.Subscribe(observer);
        }

        public void Trigger(TInput input)
        {
            var handler = Triggered;
            handler?.Invoke(input);
        }
    }
}
