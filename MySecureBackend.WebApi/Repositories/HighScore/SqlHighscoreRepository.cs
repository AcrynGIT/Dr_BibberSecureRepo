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

            var existing = await SelectAsync(highscore.UserId);
            if (existing != null)
                throw new InvalidOperationException("Score bestaat al. Gebruik PUT om te updaten.");

            await sqlConnection.ExecuteAsync(
                "INSERT INTO [Highscores] (UserId, Score, UpdatedAt) VALUES (@UserId, @Score, @UpdatedAt)",
                highscore
            );
        }

        public async Task<Highscore?> SelectAsync(string userId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QuerySingleOrDefaultAsync<Highscore>(
                "SELECT * FROM [Highscores] WHERE UserId = @userId",
                new { userId }
            );
        }

        public async Task<IEnumerable<Highscore>> SelectAsync()
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QueryAsync<Highscore>(
                "SELECT * FROM [Highscores] ORDER BY UpdatedAt DESC"
            );
        }

        public async Task UpdateAsync(Highscore highscore)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            var existing = await SelectAsync(highscore.UserId);
            if (existing == null)
                throw new InvalidOperationException("Score bestaat nog niet. Gebruik POST om aan te maken.");

            await sqlConnection.ExecuteAsync(
                "UPDATE [Highscores] SET Score = @Score, UpdatedAt = @UpdatedAt WHERE UserId = @UserId",
                highscore
            );
        }

        public async Task DeleteAsync(string userId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync(
                "DELETE FROM [Highscores] WHERE UserId = @userId",
                new { userId }
            );
        }
    }
}