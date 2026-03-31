using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class MemoryUserAvatarRepository : IUserAvatarRepository
    {
        private static List<UserAvatar> userAvatars = new();

        public Task DeleteAsync(string userId)
        {
            userAvatars.RemoveAll(x => x.UserId == userId);
            return Task.CompletedTask;
        }

        public Task InsertAsync(UserAvatar userAvatar)
        {
            userAvatars.Add(userAvatar);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<UserAvatar>> SelectAsync()
        {
            return Task.FromResult(userAvatars.AsEnumerable());
        }

        public Task<UserAvatar?> SelectAsync(string userId)
        {
            return Task.FromResult(userAvatars.SingleOrDefault(x => x.UserId == userId));
        }

        public async Task UpdateAsync(UserAvatar userAvatar)
        {
            await DeleteAsync(userAvatar.UserId);
            await InsertAsync(userAvatar);
        }
    }
}