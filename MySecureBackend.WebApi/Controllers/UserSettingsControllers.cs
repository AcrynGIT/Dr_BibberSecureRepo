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
    private readonly IUserSettingRepository _repository;
    private readonly IAuthenticationService _authenticationService;

    public UserSettingsController(IUserSettingRepository repository, IAuthenticationService authenticationService)
    {
        _repository = repository;
        _authenticationService = authenticationService;
    }

    // GET /UserSettings
    [HttpGet(Name = "GetUserSettings")]
    public async Task<ActionResult<IEnumerable<UserSetting>>> GetAsync()
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        var allSettings = await _repository.SelectAsync();
        var userSettings = allSettings.Where(x => x.UserId == currentUserId);

        return Ok(userSettings);
    }

    // POST /UserSettings
    [HttpPost(Name = "AddUserSetting")]
    public async Task<ActionResult<UserSetting>> AddAsync(UserSetting userSetting)
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        userSetting.UserId = currentUserId;
        await _repository.InsertAsync(userSetting);

        return CreatedAtRoute("GetUserSettings", null, userSetting);
    }

    // PUT /UserSettings/{kindVoornaam}/{kindAchternaam}
    [HttpPut("{kindVoornaam}/{kindAchternaam}", Name = "UpdateUserSetting")]
    public async Task<ActionResult<UserSetting>> UpdateAsync(string kindVoornaam, string kindAchternaam, UserSetting userSetting)
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        var existing = await _repository.SelectAsync(currentUserId, kindVoornaam, kindAchternaam);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = $"User setting for {kindVoornaam} {kindAchternaam} not found" });

        userSetting.UserId = currentUserId;
        userSetting.KindVoornaam = kindVoornaam;
        userSetting.KindAchternaam = kindAchternaam;

        await _repository.UpdateAsync(userSetting);
        return Ok(userSetting);
    }

    // DELETE /UserSettings/{kindVoornaam}/{kindAchternaam}
    [HttpDelete("{kindVoornaam}/{kindAchternaam}", Name = "DeleteUserSetting")]
    public async Task<ActionResult> DeleteAsync(string kindVoornaam, string kindAchternaam)
    {
        var currentUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return Forbid();

        var existing = await _repository.SelectAsync(currentUserId, kindVoornaam, kindAchternaam);
        if (existing == null)
            return NotFound(new ProblemDetails { Detail = $"User setting for {kindVoornaam} {kindAchternaam} not found" });

        await _repository.DeleteAsync(currentUserId, kindVoornaam, kindAchternaam);
        return Ok();
    }
}