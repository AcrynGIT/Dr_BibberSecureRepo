using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;

namespace MySecureBackend.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Environment2DController : ControllerBase
    {
        private readonly IEnvironment2DRepository _repo;

        public Environment2DController(IEnvironment2DRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll() =>
            Ok(_repo.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _repo.GetById(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public IActionResult Create(Environment2D env)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = _repo.Add(env);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Environment2D env)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return _repo.Update(id, env) ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) =>
            _repo.Delete(id) ? NoContent() : NotFound();
    }
}
