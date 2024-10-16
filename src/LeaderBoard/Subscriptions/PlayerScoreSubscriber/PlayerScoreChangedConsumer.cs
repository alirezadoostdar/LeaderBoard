﻿using StackExchange.Redis;

namespace LeaderBoard.Subscriptions.PlayerScoreSubscriber;

public class PlayerScoreChangedConsumer(
    ScoreService scoreService,
    SortedInMemoryDatabase sortedSet,
    LeaderBoardDbContext context) : IConsumer<PlayerScoreChangedEvent>
{
    private readonly ScoreService _scoreService = scoreService;
    private readonly LeaderBoardDbContext _context = context;
    private readonly SortedInMemoryDatabase _sortedSet = sortedSet;

    public async Task Consume(ConsumeContext<PlayerScoreChangedEvent> context)
    {
        // get data from consumer
        var message = context.Message;
        PlayerScore playerScore = new PlayerScore() { Score = message.score,Username =message.PlayerUsername};

        var item = await _context.PlayerScores.FirstOrDefaultAsync(f => f.Username == message.PlayerUsername);
        if (item is not null)
        {
            item.Score = message.score;
        }
        else
        {
            _context.PlayerScores.Add(playerScore);
        }
        await _context.SaveChangesAsync();

        //Update sorted set
        _sortedSet.AddItem(playerScore);

        //Add and Update to redis
        await _scoreService.Add(PlayerScore.RedisKey, playerScore.Username, playerScore.Score);

    }
}
