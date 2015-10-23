namespace Flower
{
    using Flower.Works;

    public interface IWorkDecoratorFactory
    {
        IExecutableActionWork Decorate(IExecutableActionWork work);
        IExecutableActionWork<TInput> Decorate<TInput>(IExecutableActionWork<TInput> work);
        IExecutableFuncWork<TInput, TOutput> Decorate<TInput, TOutput>(IExecutableFuncWork<TInput, TOutput> work);
    }
}
