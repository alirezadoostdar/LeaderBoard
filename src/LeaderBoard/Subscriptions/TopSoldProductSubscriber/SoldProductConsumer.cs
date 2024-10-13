using LeaderBoard.Models;
using MassTransit;
using StackExchange.Redis;

namespace LeaderBoard.Subscriptions.TopSoldProductSubscriber
{
    public class SoldProductConsumer(
        ScoreService scoreService,
        SortedInMemoryDatabase sortedSet,
        LeaderBoardDbContext context) : IConsumer<SoldProductEvent>
    {
        private readonly ScoreService _scoreService = scoreService;
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

            //Add and Update to redis
            await _scoreService.Increment(MostSoldProduct.RedisKey, soldProdut.CatalogId);
        }
    }
}
