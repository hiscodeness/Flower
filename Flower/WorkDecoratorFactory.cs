namespace Flower
{
    using Flower.Works;

    public abstract class WorkDecoratorFactory : IWorkDecoratorFactory
    {
        public virtual IExecutableActionWork Decorate(IExecutableActionWork work)
        {
            return work;
        }

        public virtual IExecutableActionWork<TInput> Decorate<TInput>(IExecutableActionWork<TInput> work)
        {
            return work;
        }

        public virtual IExecutableFuncWork<TInput, TOutput> Decorate<TInput, TOutput>(IExecutableFuncWork<TInput, TOutput> work)
        {
            return work;
        }
    }
}
