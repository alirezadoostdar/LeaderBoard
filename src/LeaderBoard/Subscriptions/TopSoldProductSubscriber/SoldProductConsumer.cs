using MassTransit;

namespace LeaderBoard.Subscriptions.TopSoldProductSubscriber
{
    public class SoldProductConsumer(LeaderBoardDbContext context) : IConsumer<SoldProductEvent>
    {
        public readonly LeaderBoardDbContext _context = context;
        public async Task Consume(ConsumeContext<SoldProductEvent> context)
        {
            var message = context.Message;
            var item = await _context.MostSoldProducts.FirstOrDefaultAsync(f => f.CatalogId == message.slug);
            if (item is not null)
            {
                item.Score++;
            }
            else
            {
                _context.MostSoldProducts.Add(new MostSoldProduct
                {
                    CatalogId = message.slug,
                    Score = 1
                });
            }
            await _context.SaveChangesAsync();
        }
    }
}
