using System;
using Autofac.Features.OwnedInstances;
using Flower.Autofac;

// ReSharper disable once CheckNamespace Extension methods can be located in the extended namespace
namespace Flower
{
    public static class CreateOwnedInstanceExtensionMethods
    {
        public static Func<IScope<T>> Scope<T>(this Func<Owned<T>> resolveOwned)
        {
            return () => new OwnedScope<T>(resolveOwned);
        }
    }
}
