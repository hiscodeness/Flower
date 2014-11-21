using System;
using Autofac.Features.OwnedInstances;

namespace Flower.Autofac
{
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
