using Microsoft.Data.SqlClient;
using Puissance4.Domain.Entities;
using Dapper;

namespace Puissance4.DAL.Repositories
{
    public class GameRepository(SqlConnection connection)
    {
        public void Add(Game game)
        {
            string sql = @"INSERT INTO [Game](
                               [Id], 
                               [RedPlayerId], 
                               [YellowPlayerId], 
                               [Winner], 
                               [SerializedGrid]
                           )
                           VALUES (
                               @id, 
                               @redPlayerId, 
                               @yellowPlayerId, 
                               @winner, 
                               @serializedGrid
                           )";
            connection.Execute(sql, game);
        }

        public IEnumerable<Game> FindByPlayerId(int playerId)
        {
            string sql = @"SELECT * FROM [Game] WHERE [RedPlayerId] = @playerId OR [YellowPlayerId] = @playerId";
            return connection.Query<Game>(sql, new { playerId });
        }
    }
}
