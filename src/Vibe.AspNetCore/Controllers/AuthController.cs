using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vibe.AspNetCore.Authentication;

namespace Vibe.AspNetCore.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        // Demo-only credential check. Replace with a real user store before shipping.
        if (request.Username != "demo" || request.Password != "password")
        {
            return Unauthorized();
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresMinutes);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: [new Claim(JwtRegisteredClaimNames.Sub, request.Username)],
            expires: expiresAt,
            signingCredentials: credentials);

        return Ok(new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresAt));
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<UserInfoResponse> Me()
    {
        var username = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.Identity?.Name;

        if (username is null)
        {
            return Unauthorized();
        }

        var claims = User.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToArray());

        return Ok(new UserInfoResponse(username, claims));
    }
}

public record LoginRequest(string Username, string Password);

public record LoginResponse(string AccessToken, DateTime ExpiresAt);

public record UserInfoResponse(string Username, IDictionary<string, string[]> Claims);
