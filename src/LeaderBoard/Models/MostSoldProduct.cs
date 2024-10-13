using Microsoft.AspNetCore.Components;
using StackExchange.Redis;

namespace LeaderBoard.Models;

public class MostSoldProduct:BaseScoreType
{
    public const string RedisKey = "SoldProduct";
    public int Id { get; set; }
    
    [Elemen]
    public string CatalogId { get; set; } = null!;
}
