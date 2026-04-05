using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    [HttpGet(Name = "GetHighscores")]
    public async Task<ActionResult<IEnumerable<Highscore>>> GetAsync()
    {
        var highscores = await _repository.SelectAsync();
        return Ok(highscores);
    }

    [HttpPost(Name = "AddHighscores")]
    public async Task<ActionResult<Highscore>> Add(Highscore highscore)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        highscore.UserId = userId;
        await _repository.InsertAsync(highscore);

        return Created("/highscores", highscore);
    }

    [HttpPut(Name = "UpdateHighscores")]
    public async Task<ActionResult<Highscore>> Update(Highscore highscore)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        var existing = await _repository.SelectAsync(userId);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = "Score bestaat nog niet. Gebruik POST om aan te maken" });

        highscore.UserId = userId;
        await _repository.UpdateAsync(highscore);

        return Ok(highscore);
    }

    [HttpDelete(Name = "DeleteHighscores")]
    public async Task<ActionResult> Delete()
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        await _repository.DeleteAsync(userId);
        return Ok();
    }
}