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
public class UserSettingsController : ControllerBase
{
    private readonly IUserSettingsRepository _repository;
    private readonly IAuthenticationService _authenticationService;

    public UserSettingsController(IUserSettingsRepository repository, IAuthenticationService authenticationService)
    {
        _repository = repository;
        _authenticationService = authenticationService;
    }

    
    [HttpGet(Name = "GetUserSettings")]
    public async Task<ActionResult<UserSettings>> GetAsync()
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        var settings = await _repository.SelectAsync(userId);

        if (settings == null)
            return NotFound(new ProblemDetails { Detail = "UserSettings not found" });

        return Ok(settings);
    }

    
    [HttpPost(Name = "CreateUserSettings")]
    public async Task<ActionResult<UserSettings>> CreateAsync(UserSettings settings)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        var existing = await _repository.SelectAsync(userId);
        if (existing != null)
            return Conflict(new ProblemDetails { Detail = "UserSettings already exist" });

        settings.UserId = userId;
        settings.UpdatedAt = DateTime.UtcNow;

        await _repository.InsertAsync(settings);

        return CreatedAtRoute("GetUserSettings", null, settings);
    }

    
    [HttpPut(Name = "UpdateUserSettings")]
    public async Task<ActionResult<UserSettings>> UpdateAsync(UserSettings settings)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        var existing = await _repository.SelectAsync(userId);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = "UserSettings not found" });

        settings.UserId = userId;
        settings.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(settings);

        return Ok(settings);
    }

    
    [HttpDelete(Name = "DeleteUserSettings")]
    public async Task<ActionResult> DeleteAsync()
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        var existing = await _repository.SelectAsync(userId);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = "UserSettings not found" });

        await _repository.DeleteAsync(userId);

        return Ok();
    }
}