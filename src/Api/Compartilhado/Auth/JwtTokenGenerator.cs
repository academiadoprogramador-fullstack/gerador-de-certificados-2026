using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GeradorCertificadosOnline.Api.Compartilhado.Auth;

/// <summary>
/// Gera o access token JWT devolvido por POST /auth/login.
/// </summary>
public sealed class JwtTokenGenerator(IOptions<JwtOptions> jwtOptions) : IEmissorTokens
{
    private readonly JwtOptions _options = jwtOptions.Value;

    public TokenDto CriarToken(Guid usuarioId, string email)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var expiraEm = TimeSpan.FromMinutes(_options.ExpiresInMinutes);
        var dataExpiracaoEmUtc = DateTime.UtcNow.Add(expiraEm);
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: dataExpiracaoEmUtc,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenDto(usuarioId, accessToken, dataExpiracaoEmUtc);
    }
}
