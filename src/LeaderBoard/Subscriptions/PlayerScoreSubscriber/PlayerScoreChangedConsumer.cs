namespace LeaderBoard.Subscriptions.PlayerScoreSubscriber;

public class PlayerScoreChangedConsumer(LeaderBoardDbContext context) : IConsumer<PlayerScoreChangedEvent>
{
    private readonly LeaderBoardDbContext _context = context;
    public async Task Consume(ConsumeContext<PlayerScoreChangedEvent> context)
    {
        // save on the set
        var message = context.Message;
        var item = await _context.PlayerScores.FirstOrDefaultAsync(f => f.Username == message.PlayerUsername);
        if (item is not null)
        {
            item.Score = message.score;
        }
        else
        {
            _context.PlayerScores.Add(new Models.PlayerScore
            {
                Score = message.score,
                Username = message.PlayerUsername,
            });
        }
        await _context.SaveChangesAsync();
    }
}
