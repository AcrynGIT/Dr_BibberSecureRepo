using MySecureBackend.WebApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySecureBackend.WebApi.Repositories
{
    public class MemoryUserAvatarRepository : IUserAvatarRepository
    {
        private static List<UserAvatar> userAvatars = new();

        public Task InsertAsync(UserAvatar userAvatar)
        {
            if (userAvatars.Any(x => x.UserId == userAvatar.UserId))
                throw new InvalidOperationException("Avatar bestaat al. Gebruik PUT om te updaten.");

            userAvatars.Add(userAvatar);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string userId)
        {
            userAvatars.RemoveAll(x => x.UserId == userId);
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

        public Task UpdateAsync(UserAvatar userAvatar)
        {
            var existing = userAvatars.SingleOrDefault(x => x.UserId == userAvatar.UserId);
            if (existing == null)
                throw new InvalidOperationException("Avatar bestaat nog niet. Gebruik POST om aan te maken.");

            existing.AvatarId = userAvatar.AvatarId;
            return Task.CompletedTask;
        }
    }
}