using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;

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
            await sqlConnection.ExecuteAsync(
                "INSERT INTO [UserAvatars] (UserId, AvatarId, SelectedAt) VALUES (@UserId, @AvatarId, @SelectedAt)",
                userAvatar
            );
        }

        public async Task<UserAvatar?> SelectAsync(string userId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            return await sqlConnection.QuerySingleOrDefaultAsync<UserAvatar>(
                "SELECT * FROM [UserAvatars] WHERE UserId = @userId", new { userId }
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
            await sqlConnection.ExecuteAsync(
                "UPDATE [UserAvatars] SET AvatarId = @AvatarId, SelectedAt = @SelectedAt WHERE UserId = @UserId",
                userAvatar
            );
        }

        public async Task DeleteAsync(string userId)
        {
            using var sqlConnection = new SqlConnection(sqlConnectionString);
            await sqlConnection.ExecuteAsync("DELETE FROM [UserAvatars] WHERE UserId = @userId", new { userId });
        }
    }
}