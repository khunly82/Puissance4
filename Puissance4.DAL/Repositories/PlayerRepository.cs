using Dapper;
using Microsoft.Data.SqlClient;
using Puissance4.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puissance4.DAL.Repositories
{
    public class PlayerRepository(SqlConnection connection)
    {
        public void Add(Player player)
        {
            string sql = @"INSERT INTO Player(Username, Password) VALUES (@username, @password)";
            connection.Execute(sql, player);
        }

        public Player? FindById(int? userId)
        {
            string sql = @"SELECT * FROM Player WHERE Id LIKE @id";
            return connection.QueryFirstOrDefault<Player>(sql, new { Id = userId });
        }

        public Player? FindByUsername(string username)
        {
            string sql = @"SELECT * FROM Player WHERE Username LIKE @username";
            return connection.QueryFirstOrDefault<Player>(sql, new { username });
        }
    }
}
