using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MySecureBackend.WebApi.Repositories
{
    public class SqlUserAvatarRepository : IUserAvatarRepository
    {
        private readonly string sqlConnectionString;

        public SqlUserAvatarRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task InsertAsync(UserAvatar userAvatar)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            var existing = await SelectAsync(userAvatar.UserId);
            if (existing != null)
                throw new InvalidOperationException("Avatar bestaat al. Gebruik PUT om te updaten.");

            await sqlConnection.ExecuteAsync(
                "INSERT INTO [UserAvatars] (UserId, AvatarId) VALUES (@UserId, @AvatarId)",
                userAvatar
            );
        }

        public async Task<UserAvatar?> SelectAsync(string userId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QuerySingleOrDefaultAsync<UserAvatar>(
                "SELECT * FROM [UserAvatars] WHERE UserId = @userId",
                new { userId }
            );
        }

        public async Task<IEnumerable<UserAvatar>> SelectAsync()
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QueryAsync<UserAvatar>("SELECT * FROM [UserAvatars]");
        }

        public async Task UpdateAsync(UserAvatar userAvatar)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            var existing = await SelectAsync(userAvatar.UserId);
            if (existing == null)
                throw new InvalidOperationException("Avatar bestaat nog niet. Gebruik POST om aan te maken.");

            await sqlConnection.ExecuteAsync(
                "UPDATE [UserAvatars] SET AvatarId = @AvatarId WHERE UserId = @UserId",
                userAvatar
            );
        }

        public async Task DeleteAsync(string userId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync(
                "DELETE FROM [UserAvatars] WHERE UserId = @userId",
                new { userId }
            );
        }
    }
}