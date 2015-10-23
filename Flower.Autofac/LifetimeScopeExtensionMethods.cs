 // ReSharper disable once CheckNamespace Extension methods can be in original namespace
namespace Autofac
{
    using System;
    using Features.OwnedInstances;
    using Flower;

    public static class LifetimeScopeExtensionMethods
    {
        public static Func<IScope<TWorker>> ResolveFactory<TWorker>(this ILifetimeScope lifetimeScope)
        {
            return lifetimeScope.Resolve<Func<Owned<TWorker>>>().Scope();
        }
    }
}
