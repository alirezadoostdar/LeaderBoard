namespace LeaderBoard.Models;

public class PlayerScore:BaseScoreType
{
    public const string RedisKey = "PlayerScore";
    public int Id { get; set; }
    public string Username { get; set; } = null!;
}
