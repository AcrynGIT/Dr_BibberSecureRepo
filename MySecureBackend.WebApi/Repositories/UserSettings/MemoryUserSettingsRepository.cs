using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class MemoryUserSettingRepository : IUserSettingRepository
    {
        private static List<UserSetting> userSettings = new();

        public Task DeleteAsync(string userId, string kindVoornaam, string kindAchternaam)
        {
            userSettings.RemoveAll(x => x.UserId == userId && x.KindVoornaam == kindVoornaam && x.KindAchternaam == kindAchternaam);
            return Task.CompletedTask;
        }

        public Task InsertAsync(UserSetting userSetting)
        {
            userSettings.Add(userSetting);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<UserSetting>> SelectAsync()
        {
            return Task.FromResult(userSettings.AsEnumerable());
        }

        public Task<UserSetting?> SelectAsync(string userId, string kindVoornaam, string kindAchternaam)
        {
            return Task.FromResult(userSettings.SingleOrDefault(x =>
                x.UserId == userId && x.KindVoornaam == kindVoornaam && x.KindAchternaam == kindAchternaam));
        }

        public async Task UpdateAsync(UserSetting userSetting)
        {
            await DeleteAsync(userSetting.UserId, userSetting.KindVoornaam, userSetting.KindAchternaam);
            await InsertAsync(userSetting);
        }
    }
}