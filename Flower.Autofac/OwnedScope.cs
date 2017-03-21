namespace Flower.Autofac
{
    using System;
    using global::Autofac.Features.OwnedInstances;

    public sealed class OwnedScope<T> : IScope<T>
    {
        private readonly Owned<T> owned;

        public OwnedScope(Func<Owned<T>> resolveOwned)
        {
            owned = resolveOwned();
            Worker = owned.Value;
        }

        public T Worker { get; private set; }

        public void Dispose()
        {
            owned.Dispose();
        }
    }
}
