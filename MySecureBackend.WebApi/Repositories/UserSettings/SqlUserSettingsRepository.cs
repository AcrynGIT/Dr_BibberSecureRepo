using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class SqlUserSettingsRepository : IUserSettingsRepository
    {
        private readonly string sqlConnectionString;

        public SqlUserSettingsRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<UserSettings?> SelectAsync(string userId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            return await sqlConnection.QuerySingleOrDefaultAsync<UserSettings>(
                "SELECT * FROM UserSettings WHERE UserId = @userId",
                new { userId }
            );
        }

        public async Task InsertAsync(UserSettings settings)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            await sqlConnection.ExecuteAsync(@"
                INSERT INTO UserSettings
                (UserId, KindVoornaam, KindAchternaam, KindLeeftijd, ArtsNaam, BehandelingType, Behandeldatum, UpdatedAt)
                VALUES
                (@UserId, @KindVoornaam, @KindAchternaam, @KindLeeftijd, @ArtsNaam, @BehandelingType, @Behandeldatum, @UpdatedAt)",
                settings
            );
        }

        public async Task UpdateAsync(UserSettings settings)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            await sqlConnection.ExecuteAsync(@"
                UPDATE UserSettings SET
                    KindVoornaam = @KindVoornaam,
                    KindAchternaam = @KindAchternaam,
                    KindLeeftijd = @KindLeeftijd,
                    ArtsNaam = @ArtsNaam,
                    BehandelingType = @BehandelingType,
                    Behandeldatum = @Behandeldatum,
                    UpdatedAt = @UpdatedAt
                WHERE UserId = @UserId",
                settings
            );
        }

        public async Task DeleteAsync(string userId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);

            await sqlConnection.ExecuteAsync(
                "DELETE FROM UserSettings WHERE UserId = @userId",
                new { userId }
            );
        }
    }
}