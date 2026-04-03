using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IUserSettingsRepository
    {
        Task<UserSettings?> SelectAsync(string userId);
        Task InsertAsync(UserSettings settings);
        Task UpdateAsync(UserSettings settings);
        Task DeleteAsync(string userId);
    }
}