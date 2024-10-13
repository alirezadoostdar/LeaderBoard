using LeaderBoard.Models;
using StackExchange.Redis;

namespace LeaderBoard;

public class ScoreService(IConnectionMultiplexer connectionMultiplexer)
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    public async Task Add(string topic,string memeberId, int score)
    {
        await _database.SortedSetAddAsync(PlayerScore.RedisKey, memeberId, score);
    }

    public async Task Increment(string redisKey ,string slug)
    {
        await _database.SortedSetIncrementAsync(redisKey, slug, 1);
    }

    public async Task<IEnumerable<T>> GetTop<T>(string topic, int k) where T : BaseScoreType
    {
        var items = await _database.SortedSetRangeByRankWithScoresAsync(topic,0,k - 1,Order.Descending);

        var models = new List<T>(); 
        foreach (var item in items)
        {
            models.Add(item.ToModel<T>());
        }

        return models;
    }
}

public static class SortedSetEntryExtensions
{
    public static T ToModel<T>(this SortedSetEntry entry) where T : BaseScoreType
    {
        var model = Activator.CreateInstance<T>();
        model.Score = Convert.ToInt32(entry.Score);

        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var attribute = Attribute.GetCustomAttribute(property, typeof(ElemenAttribute));

            if(attribute is not null && property.CanWrite)
                property.SetValue(model,entry.Element.ToString());
        }

        return model;
    }
}
