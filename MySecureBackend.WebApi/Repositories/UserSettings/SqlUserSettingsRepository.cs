using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class SqlUserSettingRepository : IUserSettingRepository
    {
        private readonly string sqlConnectionString;

        public SqlUserSettingRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task InsertAsync(UserSetting userSetting)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync(
                @"INSERT INTO [UserSettings]
                  (UserId, KindVoornaam, KindAchternaam, KindLeeftijd, ArtsNaam, BehandelingType, Behandeldatum)
                  VALUES
                  (@UserId, @KindVoornaam, @KindAchternaam, @KindLeeftijd, @ArtsNaam, @BehandelingType, @Behandeldatum)",
                userSetting
            );
        }

        public async Task<UserSetting?> SelectAsync(string userId, string kindVoornaam, string kindAchternaam)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QuerySingleOrDefaultAsync<UserSetting>(
                @"SELECT * FROM [UserSettings] 
                  WHERE UserId = @userId AND KindVoornaam = @kindVoornaam AND KindAchternaam = @kindAchternaam",
                new { userId, kindVoornaam, kindAchternaam }
            );
        }

        public async Task<IEnumerable<UserSetting>> SelectAsync()
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QueryAsync<UserSetting>("SELECT * FROM [UserSettings]");
        }

        public async Task UpdateAsync(UserSetting userSetting)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync(
                @"UPDATE [UserSettings]
                  SET KindLeeftijd = @KindLeeftijd,
                      ArtsNaam = @ArtsNaam,
                      BehandelingType = @BehandelingType,
                      Behandeldatum = @Behandeldatum
                  WHERE UserId = @UserId AND KindVoornaam = @KindVoornaam AND KindAchternaam = @KindAchternaam",
                userSetting
            );
        }

        public async Task DeleteAsync(string userId, string kindVoornaam, string kindAchternaam)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync(
                @"DELETE FROM [UserSettings]
                  WHERE UserId = @userId AND KindVoornaam = @kindVoornaam AND KindAchternaam = @kindAchternaam",
                new { userId, kindVoornaam, kindAchternaam }
            );
        }
    }
}