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

    [HttpGet(Name = "GetHighscore")]
    public async Task<ActionResult<IEnumerable<Highscore>>> GetAll()
    {
        var highscores = await _repository.SelectAsync();
        return Ok(highscores);
    }

    [HttpPost(Name = "PostHighscore")]
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

    [HttpPut(Name = "UpdateHighscore")]
    public async Task<ActionResult<Highscore>> Update(Highscore highscore)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        var existing = await _repository.SelectAsync(userId);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = "Score bestaat nog niet. Gebruik POST om aan te maken" });

        highscore.UserId = userId;
        highscore.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(highscore);

        return Ok(highscore);
    }

    [HttpDelete(Name = "DeleteHighscore")]
    public async Task<ActionResult> Delete()
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        await _repository.DeleteAsync(userId);
        return Ok();
    }
}