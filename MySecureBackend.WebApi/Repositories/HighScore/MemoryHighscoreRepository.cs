using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class MemoryHighscoreRepository : IHighscoreRepository
    {
        private static List<Highscore> highscores = new();

        public Task DeleteAsync(string userId, string gameName)
        {
            highscores.RemoveAll(x => x.UserId == userId && x.GameName == gameName);
            return Task.CompletedTask;
        }

        public Task InsertAsync(Highscore highscore)
        {
            highscores.Add(highscore);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Highscore>> SelectAsync()
        {
            return Task.FromResult(highscores.AsEnumerable());
        }

        public Task<Highscore?> SelectAsync(string userId, string gameName)
        {
            return Task.FromResult(highscores.SingleOrDefault(x => x.UserId == userId && x.GameName == gameName));
        }

        public async Task UpdateAsync(Highscore highscore)
        {
            await DeleteAsync(highscore.UserId, highscore.GameName);
            await InsertAsync(highscore);
        }
    }
}