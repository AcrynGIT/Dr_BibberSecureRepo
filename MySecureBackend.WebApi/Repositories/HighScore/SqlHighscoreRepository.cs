using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class SqlHighscoreRepository : IHighscoreRepository
    {
        private readonly string sqlConnectionString;

        public SqlHighscoreRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task InsertAsync(Highscore highscore)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync(
                "INSERT INTO [Highscores] (UserId, GameName, Score, UpdatedAt) VALUES (@UserId, @GameName, @Score, @UpdatedAt)",
                highscore
            );
        }

        public async Task<Highscore?> SelectAsync(string userId, string gameName)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QuerySingleOrDefaultAsync<Highscore>(
                "SELECT * FROM [Highscores] WHERE UserId = @userId AND GameName = @gameName",
                new { userId, gameName }
            );
        }

        public async Task<IEnumerable<Highscore>> SelectAsync()
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QueryAsync<Highscore>("SELECT * FROM [Highscores]");
        }

        public async Task UpdateAsync(Highscore highscore)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync(
                "UPDATE [Highscores] SET Score = @Score, UpdatedAt = @UpdatedAt WHERE UserId = @UserId AND GameName = @GameName",
                highscore
            );
        }

        public async Task DeleteAsync(string userId, string gameName)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync(
                "DELETE FROM [Highscores] WHERE UserId = @userId AND GameName = @gameName",
                new { userId, gameName }
            );
        }
    }
}