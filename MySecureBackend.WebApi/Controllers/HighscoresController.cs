using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("highscores")]
[Consumes("application/json")]
[Produces("application/json")]
public class HighscoresController : ControllerBase
{
    private readonly IHighscoreRepository _repository;
    private readonly IAuthenticationService _authenticationService;

    public HighscoresController(IHighscoreRepository repository, IAuthenticationService authenticationService)
    {
        _repository = repository;
        _authenticationService = authenticationService;
    }

    // GET /highscores -> leaderboard
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Highscore>>> GetAll()
    {
        var highscores = await _repository.SelectAsync();
        return Ok(highscores);
    }

    // POST /highscores -> nieuwe score toevoegen
    [HttpPost]
    public async Task<ActionResult<Highscore>> Add(Highscore highscore)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        highscore.UserId = userId;
        highscore.UpdatedAt = DateTime.UtcNow;

        await _repository.InsertAsync(highscore);

        return Created("/highscores", highscore);
    }

    // PUT /highscores -> bestaande score updaten
    [HttpPut]
    public async Task<ActionResult<Highscore>> Update(Highscore highscore)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        var existing = await _repository.SelectAsync(userId);

        highscore.UserId = userId;
        highscore.UpdatedAt = DateTime.UtcNow;

        if (existing == null)
        {
            return NotFound(new ProblemDetails { Detail = "Score bestaat nog niet, gebruik POST om aan te maken" });
        }

        await _repository.UpdateAsync(highscore);

        return Ok(highscore);
    }

    // DELETE /highscores -> delete score
    [HttpDelete]
    public async Task<ActionResult> Delete()
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        await _repository.DeleteAsync(userId);
        return Ok();
    }
}