using System.Linq;

namespace LeaderBoard.Database;

public class SortedInMemoryDatabase
{
    private Dictionary<string, SortedSet<Type>> _sets;

    private SortedSet<PlayerScore> _playerScores = new SortedSet<PlayerScore>(new BaseScoreTypeCpmparer());
    private SortedSet<MostSoldProduct> _mostSoldProducts = new SortedSet<MostSoldProduct>(new BaseScoreTypeCpmparer());

    public SortedSet<PlayerScore> PlayerScores => _playerScores;
    public SortedSet<MostSoldProduct> MostSoldProducts => _mostSoldProducts;
    public SortedInMemoryDatabase( )
    {

    }

    public void AddItem<TModel>(TModel model ) where TModel : BaseScoreType
    {
       if (model is PlayerScore player)
        {
           var item =  _playerScores.Where(x=>x.Username==player.Username).FirstOrDefault();
            if (item is not null)
                _playerScores.Remove(item);

            _playerScores.Add(player);
        }
        else if (model is MostSoldProduct catalog)
        {
            var item = _mostSoldProducts.FirstOrDefault(catalog);
            if (item is not null)
            {
                _mostSoldProducts.Remove(item);
                item.Score++;
            }

            _mostSoldProducts.Add(catalog);
        }
    }

}
