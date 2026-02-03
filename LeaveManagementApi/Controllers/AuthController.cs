using LeaveManagementApi.Authorization;
using LeaveManagementApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LeaveManagementApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserStore _userStore;
    private readonly IJwtTokenService _tokenService;
    private readonly AuthSettings _settings;

    public AuthController(IUserStore userStore, IJwtTokenService tokenService, IOptions<AuthSettings> settings)
    {
        _userStore = userStore;
        _tokenService = tokenService;
        _settings = settings.Value;
    }

    [HttpPost("login")]
    public ActionResult<LoginResponseDto> Login(LoginRequestDto dto)
    {
        var user = _userStore.ValidateUser(dto.Username, dto.Password);
        if (user is null)
        {
            return Unauthorized();
        }

        var token = _tokenService.CreateToken(user);
        return Ok(new LoginResponseDto(token, "Bearer", _settings.TokenLifetimeMinutes * 60));
    }
}
