 // ReSharper disable once CheckNamespace Extension methods can be in original namespace
namespace Autofac
{
    using System;
    using Features.OwnedInstances;
    using Flower;

    public static class ComponentContextExtensionMethods
    {
        public static Func<IScope<TWorker>> ResolveFactory<TWorker>(this IComponentContext componentContext)
        {
            return componentContext.Resolve<Func<Owned<TWorker>>>().Scope();
        }
    }
}
