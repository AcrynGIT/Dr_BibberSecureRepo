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
[Route("useravatars")]
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

    [HttpGet(Name = "GetUseravatars")]
    public async Task<ActionResult<UserAvatar>> GetAsync()
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        var avatar = await _repository.SelectAsync(userId);
        if (avatar == null)
            return NotFound(new ProblemDetails { Detail = "User avatar not found" });

        return Ok(avatar);
    }

    [HttpPost(Name = "AddUserAvatars")]
    public async Task<ActionResult<UserAvatar>> Add(UserAvatar avatar)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        avatar.UserId = userId;
        await _repository.InsertAsync(avatar);

        return Created("/useravatars", avatar);
    }

    [HttpPut(Name = "UpdateUserAvatars")]
    public async Task<ActionResult<UserAvatar>> Update(UserAvatar avatar)
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        var existing = await _repository.SelectAsync(userId);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = "Avatar bestaat nog niet. Gebruik POST om aan te maken" });

        avatar.UserId = userId;
        await _repository.UpdateAsync(avatar);

        return Ok(avatar);
    }

    [HttpDelete(Name = "DeleteUserAvatars")]
    public async Task<ActionResult> Delete()
    {
        var userId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(userId))
            return Forbid();

        await _repository.DeleteAsync(userId);
        return Ok();
    }
}