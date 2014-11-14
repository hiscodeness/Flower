namespace Flower.Tests.TestDoubles
{
    public static class WorkRegistryFactory
    {
        public static WorkRegistry CreateAutoActivating()
        {
            var options = new WorkRegistryOptions(RegisterWorkBehavior.RegisterActivated);
            return new WorkRegistry(options);
        }
    }
}
