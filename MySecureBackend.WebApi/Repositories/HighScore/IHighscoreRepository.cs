using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IHighscoreRepository
    {
        Task InsertAsync(Highscore highscore);
        Task DeleteAsync(string userId);
        Task<IEnumerable<Highscore>> SelectAsync();
        Task<Highscore?> SelectAsync(string userId);
        Task UpdateAsync(Highscore highscore);
    }
}