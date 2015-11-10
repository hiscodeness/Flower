namespace Flower.Triggers
{
    using System;
    using System.Reactive.Linq;

    public class TriggerBase<TInput> : ITrigger<TInput>
    {
        private event Action<TInput> Triggered;
        private readonly IObservable<TInput> observable;

        public TriggerBase()
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
