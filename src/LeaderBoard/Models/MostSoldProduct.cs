using StackExchange.Redis;

namespace LeaderBoard.Models;

public class MostSoldProduct:BaseScoreType
{
    public const string RedisKey = "SoldProduct";
    public int Id { get; set; }
    public string CatalogId { get; set; } = null!;
}
