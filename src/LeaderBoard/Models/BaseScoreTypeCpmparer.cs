namespace LeaderBoard.Models;

public class BaseScoreTypeCpmparer : IComparer<BaseScoreType>
{
    public int Compare(BaseScoreType? x, BaseScoreType? y)
    {
        ArgumentNullException.ThrowIfNull(nameof(x));
        ArgumentNullException.ThrowIfNull(nameof(y));

        return x!.Score.CompareTo(y!.Score);
    }
}
