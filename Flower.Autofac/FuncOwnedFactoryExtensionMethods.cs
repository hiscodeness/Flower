using System;
using Autofac.Features.OwnedInstances;
using Flower.Autofac;

// ReSharper disable once CheckNamespace Extension methods can be located in the extended namespace
namespace Flower
{
    public static class FuncOwnedFactoryExtensionMethods
    {
        public static Func<IScope<T>> Scope<T>(this Func<Owned<T>> factory)
        {
            return () => new OwnedScope<T>(factory);
        }
    }
}
