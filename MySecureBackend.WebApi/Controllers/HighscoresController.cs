using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;

namespace MySecureBackend.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class HighscoresController : ControllerBase
{
    private readonly IHighscoreRepository _repository;

    public HighscoresController(IHighscoreRepository repository)
    {
        _repository = repository;
    }

    [HttpGet(Name = "GetHighscores")]
    public async Task<ActionResult<IEnumerable<Highscore>>> GetAsync()
    {
        var highscores = await _repository.SelectAsync();
        return Ok(highscores);
    }

    [HttpGet("{userId}/{gameName}", Name = "GetHighscoreById")]
    public async Task<ActionResult<Highscore>> GetByIdAsync(string userId, string gameName)
    {
        var highscore = await _repository.SelectAsync(userId, gameName);
        if (highscore == null)
            return NotFound(new ProblemDetails { Detail = $"Highscore for {userId} / {gameName} not found" });
        return Ok(highscore);
    }

    [HttpPost(Name = "AddHighscore")]
    public async Task<ActionResult<Highscore>> AddAsync(Highscore highscore)
    {
        highscore.UpdatedAt = DateTime.UtcNow;
        await _repository.InsertAsync(highscore);
        return CreatedAtRoute("GetHighscoreById",
                              new { userId = highscore.UserId, gameName = highscore.GameName },
                              highscore);
    }

    [HttpPut("{userId}/{gameName}", Name = "UpdateHighscore")]
    public async Task<ActionResult<Highscore>> UpdateAsync(string userId, string gameName, Highscore highscore)
    {
        var existing = await _repository.SelectAsync(userId, gameName);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = $"Highscore for {userId} / {gameName} not found" });

        if (userId != highscore.UserId || gameName != highscore.GameName)
            return Conflict(new ProblemDetails { Detail = "Route keys do not match body keys" });

        highscore.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(highscore);
        return Ok(highscore);
    }

    [HttpDelete("{userId}/{gameName}", Name = "DeleteHighscore")]
    public async Task<ActionResult> DeleteAsync(string userId, string gameName)
    {
        var existing = await _repository.SelectAsync(userId, gameName);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = $"Highscore for {userId} / {gameName} not found" });

        await _repository.DeleteAsync(userId, gameName);
        return Ok();
    }
}