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

        // INSERT nieuwe UserSettings
        public async Task InsertAsync(UserSettings settings)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync(
                @"INSERT INTO [UserSettings] 
                  (UserId, KindVoornaam, KindAchternaam, KindLeeftijd, ArtsNaam, BehandelingType, Behandeldatum, UpdatedAt) 
                  VALUES (@UserId, @KindVoornaam, @KindAchternaam, @KindLeeftijd, @ArtsNaam, @BehandelingType, @Behandeldatum, @UpdatedAt)",
                settings
            );
        }

        // SELECT UserSettings voor een specifieke user
        public async Task<UserSettings?> SelectAsync(string userId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QuerySingleOrDefaultAsync<UserSettings>(
                "SELECT * FROM [UserSettings] WHERE UserId = @userId",
                new { userId }
            );
        }

        // SELECT alle UserSettings (optioneel)
        public async Task<IEnumerable<UserSettings>> SelectAsync()
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QueryAsync<UserSettings>("SELECT * FROM [UserSettings]");
        }

        // UPDATE bestaande UserSettings
        public async Task UpdateAsync(UserSettings settings)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync(
                @"UPDATE [UserSettings] 
                  SET KindVoornaam = @KindVoornaam,
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

        // DELETE UserSettings voor een specifieke user
        public async Task DeleteAsync(string userId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync(
                "DELETE FROM [UserSettings] WHERE UserId = @userId",
                new { userId }
            );
        }
    }
}