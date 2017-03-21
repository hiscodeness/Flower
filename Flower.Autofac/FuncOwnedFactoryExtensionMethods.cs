// ReSharper disable once CheckNamespace Extension methods can be located in the extended namespace

namespace Flower
{
    using System;
    using Flower.Autofac;
    using global::Autofac.Features.OwnedInstances;

    public static class FuncOwnedFactoryExtensionMethods
    {
        public static Func<IScope<T>> Scope<T>(this Func<Owned<T>> factory)
        {
            return () => new OwnedScope<T>(factory);
        }
    }
}
