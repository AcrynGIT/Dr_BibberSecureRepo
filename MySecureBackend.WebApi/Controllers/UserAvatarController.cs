using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;

namespace MySecureBackend.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class UserAvatarsController : ControllerBase
{
    private readonly IUserAvatarRepository _repository;

    public UserAvatarsController(IUserAvatarRepository repository)
    {
        _repository = repository;
    }

    [HttpGet(Name = "GetUserAvatars")]
    public async Task<ActionResult<IEnumerable<UserAvatar>>> GetAsync()
    {
        var avatars = await _repository.SelectAsync();
        return Ok(avatars);
    }

    [HttpGet("{userId}", Name = "GetUserAvatarById")]
    public async Task<ActionResult<UserAvatar>> GetByIdAsync(string userId)
    {
        var avatar = await _repository.SelectAsync(userId);
        if (avatar == null)
            return NotFound(new ProblemDetails { Detail = $"UserAvatar for {userId} not found" });
        return Ok(avatar);
    }

    [HttpPost(Name = "AddUserAvatar")]
    public async Task<ActionResult<UserAvatar>> AddAsync(UserAvatar avatar)
    {
        avatar.SelectedAt = DateTime.UtcNow;
        await _repository.InsertAsync(avatar);
        return CreatedAtRoute("GetUserAvatarById", new { userId = avatar.UserId }, avatar);
    }

    [HttpPut("{userId}", Name = "UpdateUserAvatar")]
    public async Task<ActionResult<UserAvatar>> UpdateAsync(string userId, UserAvatar avatar)
    {
        var existing = await _repository.SelectAsync(userId);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = $"UserAvatar for {userId} not found" });

        if (userId != avatar.UserId)
            return Conflict(new ProblemDetails { Detail = "The UserId in the route does not match the body" });

        await _repository.UpdateAsync(avatar);
        return Ok(avatar);
    }

    [HttpDelete("{userId}", Name = "DeleteUserAvatar")]
    public async Task<ActionResult> DeleteAsync(string userId)
    {
        var existing = await _repository.SelectAsync(userId);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = $"UserAvatar for {userId} not found" });

        await _repository.DeleteAsync(userId);
        return Ok();
    }
}