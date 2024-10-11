namespace LeaderBoard.Subscriptions.PlayerScoreSubscriber;

public record PlayerScoreChangedEvent(string PlayerUsername,int score);
