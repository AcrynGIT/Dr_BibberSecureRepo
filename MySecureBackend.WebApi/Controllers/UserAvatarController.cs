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
public class UserAvatarsController : ControllerBase
{
    private readonly IUserAvatarRepository _repository;
    private readonly IAuthenticationService _authenticationService;

    public UserAvatarsController(IUserAvatarRepository repository, IAuthenticationService authenticationService)
    {
        _repository = repository;
        _authenticationService = authenticationService;
    }

    [HttpGet(Name = "GetUserAvatar")]
    public async Task<ActionResult<UserAvatar>> GetAsync()
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        var avatar = await _repository.SelectAsync(currentUserId);
        if (avatar == null)
            return NotFound(new ProblemDetails { Detail = "User avatar not found" });

        return Ok(avatar);
    }

    [HttpPost(Name = "AddUserAvatar")]
    public async Task<ActionResult<UserAvatar>> AddAsync(UserAvatar avatar)
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        avatar.UserId = currentUserId;
        avatar.SelectedAt = DateTime.UtcNow;

        await _repository.InsertAsync(avatar);

        return CreatedAtRoute("GetUserAvatar", null, avatar);
    }

    [HttpPut(Name = "UpdateUserAvatar")]
    public async Task<ActionResult<UserAvatar>> UpdateAsync(UserAvatar avatar)
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        var existing = await _repository.SelectAsync(currentUserId);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = "User avatar not found" });

        avatar.UserId = currentUserId;
        avatar.SelectedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(avatar);
        return Ok(avatar);
    }

    [HttpDelete(Name = "DeleteUserAvatar")]
    public async Task<ActionResult> DeleteAsync()
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        var existing = await _repository.SelectAsync(currentUserId);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = "User avatar not found" });

        await _repository.DeleteAsync(currentUserId);
        return Ok();
    }
}