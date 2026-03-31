using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IHighscoreRepository
    {
        Task InsertAsync(Highscore highscore);
        Task DeleteAsync(string userId, string gameName);
        Task<IEnumerable<Highscore>> SelectAsync();
        Task<Highscore?> SelectAsync(string userId, string gameName);
        Task UpdateAsync(Highscore highscore);
    }
}