using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IUserSettingRepository
    {
        Task InsertAsync(UserSetting userSetting);
        Task DeleteAsync(string userId, string kindVoornaam, string kindAchternaam);
        Task<IEnumerable<UserSetting>> SelectAsync();
        Task<UserSetting?> SelectAsync(string userId, string kindVoornaam, string kindAchternaam);
        Task UpdateAsync(UserSetting userSetting);
    }
}