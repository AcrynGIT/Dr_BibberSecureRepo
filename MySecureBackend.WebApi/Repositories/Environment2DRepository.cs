using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class Environment2DRepository : IEnvironment2DRepository
    {
        private static List<Environment2D> _items = new();
        private static int _nextId = 1;

        public IEnumerable<Environment2D> GetAll() => _items;

        public Environment2D? GetById(int id) =>
            _items.FirstOrDefault(e => e.Id == id);

        public Environment2D Add(Environment2D env)
        {
            env.Id = _nextId++;
            _items.Add(env);
            return env;
        }

        public bool Update(int id, Environment2D env)
        {
            var existing = GetById(id);
            if (existing == null) return false;

            existing.Name = env.Name;
            existing.MaxHeight = env.MaxHeight;
            existing.MaxLength = env.MaxLength;
            existing.PositionX = env.PositionX;
            existing.PositionY = env.PositionY;
            existing.ScaleX = env.ScaleX;
            existing.ScaleY = env.ScaleY;
            existing.RotationZ = env.RotationZ;
            existing.SortingLayer = env.SortingLayer;

            return true;
        }

        public bool Delete(int id)
        {
            var existing = GetById(id);
            if (existing == null) return false;

            _items.Remove(existing);
            return true;
        }
    }
}
