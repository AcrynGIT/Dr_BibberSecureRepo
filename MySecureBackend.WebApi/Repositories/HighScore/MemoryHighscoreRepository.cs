using MySecureBackend.WebApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySecureBackend.WebApi.Repositories
{
    public class MemoryHighscoreRepository : IHighscoreRepository
    {
        private static List<Highscore> highscores = new();

        public Task InsertAsync(Highscore highscore)
        {
            if (highscores.Any(x => x.UserId == highscore.UserId))
                throw new InvalidOperationException("Score bestaat al. Gebruik PUT om te updaten.");

            highscores.Add(highscore);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string userId)
        {
            highscores.RemoveAll(x => x.UserId == userId);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Highscore>> SelectAsync()
        {
            return Task.FromResult(highscores.AsEnumerable());
        }

        public Task<Highscore?> SelectAsync(string userId)
        {
            return Task.FromResult(highscores.SingleOrDefault(x => x.UserId == userId));
        }

        public Task UpdateAsync(Highscore highscore)
        {
            var existing = highscores.SingleOrDefault(x => x.UserId == highscore.UserId);
            if (existing == null)
                throw new InvalidOperationException("Score bestaat nog niet. Gebruik POST om aan te maken.");

            existing.Score = highscore.Score;
            return Task.CompletedTask;
        }
    }
}