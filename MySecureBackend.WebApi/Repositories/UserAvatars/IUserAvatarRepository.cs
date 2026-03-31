using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IUserAvatarRepository
    {
        Task InsertAsync(UserAvatar userAvatar);
        Task DeleteAsync(string userId);
        Task<IEnumerable<UserAvatar>> SelectAsync();
        Task<UserAvatar?> SelectAsync(string userId);
        Task UpdateAsync(UserAvatar userAvatar);
    }
}