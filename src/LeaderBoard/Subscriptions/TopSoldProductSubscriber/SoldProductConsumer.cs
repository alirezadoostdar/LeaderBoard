using MassTransit;

namespace LeaderBoard.Subscriptions.TopSoldProductSubscriber
{
    public class SoldProductConsumer : IConsumer<SoldProductEvent>
    {
        public Task Consume(ConsumeContext<SoldProductEvent> context)
        {
            return Task.CompletedTask;
        }
    }
}
