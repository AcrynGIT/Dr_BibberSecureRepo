using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
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

    [HttpGet(Name = "GetHighscores")]
    public async Task<ActionResult<IEnumerable<Highscore>>> GetAsync()
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        var allHighscores = await _repository.SelectAsync();

        return Ok(allHighscores);
    }

    [HttpGet("{gameName}", Name = "GetHighscoreByGame")]
    public async Task<ActionResult<Highscore>> GetByGameAsync(string gameName)
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        var highscore = await _repository.SelectAsync(currentUserId, gameName);
        if (highscore == null)
            return NotFound(new ProblemDetails { Detail = $"Highscore for {gameName} not found" });

        return Ok(highscore);
    }

    [HttpPost(Name = "AddHighscore")]
    public async Task<ActionResult<Highscore>> AddAsync(Highscore highscore)
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        highscore.UserId = currentUserId;
        highscore.UpdatedAt = DateTime.UtcNow;

        await _repository.InsertAsync(highscore);

        return CreatedAtRoute("GetHighscoreByGame",
                              new { gameName = highscore.GameName },
                              highscore);
    }

    [HttpPut("{gameName}", Name = "UpdateHighscore")]
    public async Task<ActionResult<Highscore>> UpdateAsync(string gameName, Highscore highscore)
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        var existing = await _repository.SelectAsync(currentUserId, gameName);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = $"Highscore for {gameName} not found" });

        highscore.UserId = currentUserId;
        highscore.GameName = gameName;
        highscore.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(highscore);

        return Ok(highscore);
    }

    [HttpDelete("{gameName}", Name = "DeleteHighscore")]
    public async Task<ActionResult> DeleteAsync(string gameName)
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        var existing = await _repository.SelectAsync(currentUserId, gameName);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = $"Highscore for {gameName} not found" });

        await _repository.DeleteAsync(currentUserId, gameName);
        return Ok();
    }
}