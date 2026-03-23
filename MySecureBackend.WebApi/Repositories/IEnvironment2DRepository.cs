using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IEnvironment2DRepository
    {
        IEnumerable<Environment2D> GetAll();
        Environment2D? GetById(int id);
        Environment2D Add(Environment2D env);
        bool Update(int id, Environment2D env);
        bool Delete(int id);
    }
}
