using MassTransit;

namespace LeaderBoard.Subscriptions.TopSoldProductSubscriber
{
    public class SoldProductConsumer(SortedInMemoryDatabase sortedSet, LeaderBoardDbContext context) : IConsumer<SoldProductEvent>
    {
        public readonly LeaderBoardDbContext _context = context;
        private readonly SortedInMemoryDatabase _sortedSet = sortedSet;
        public async Task Consume(ConsumeContext<SoldProductEvent> context)
        {
            var message = context.Message;
            var soldProdut = new MostSoldProduct { CatalogId = message.slug, Score = 1 };
            // Save into InMemoryDatabase
            var item = await _context.MostSoldProducts.FirstOrDefaultAsync(f => f.CatalogId == message.slug);
            if (item is not null)
            {
                item.Score++;
            }
            else
            {
                _context.MostSoldProducts.Add(soldProdut);
            }
            await _context.SaveChangesAsync();

            //Add to sorted Set list
            _sortedSet.AddItem(soldProdut);
        }
    }
}
