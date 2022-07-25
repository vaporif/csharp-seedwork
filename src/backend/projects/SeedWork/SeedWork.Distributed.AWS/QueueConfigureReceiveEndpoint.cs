namespace SeedWork.Distributed;
{
    using GreenPipes;
    using MassTransit;

    public class QueueConfigureReceiveEndpoint : IConfigureReceiveEndpoint
    {
        public void Configure(string name, IReceiveEndpointConfigurator configurator)
        {
            // Note: If you want to publish faults you should:
            // 1. Create queue for faults
            // 2. Only for fifo set message group ID. For example:
            // configurator.ConfigurePublish(c => c.AddPipeSpecification(new DelegatePipeSpecification<PublishContext>(ctx => ctx.TrySetGroupId("YOUR GROUP ID"))));
            configurator.PublishFaults = false;
            configurator.UseInMemoryOutbox();
            configurator.UseMessageRetry(c =>
            {
                // (shukur): In my opinion exponential retry is better
                c.Immediate(5);
            });
        }
    }
}
