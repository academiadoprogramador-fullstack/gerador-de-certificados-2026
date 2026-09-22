using System.Security.Claims;
using System.Text;
using GeradorCertificadosOnline.Dominio.Compartilhado.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GeradorCertificadosOnline.Api.Compartilhado.Auth;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthServices(this IServiceCollection services)
    {
        services
            .AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer), "Jwt:Issuer é obrigatório.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "Jwt:Audience é obrigatório.")
            .Validate(
                o => !string.IsNullOrWhiteSpace(o.SigningKey)
                    && Encoding.UTF8.GetByteCount(o.SigningKey) >= 32,
                "Jwt:SigningKey deve possuir ao menos 32 bytes.")
            .Validate(
                o => o.ExpiresInMinutes is > 0 and <= 35791394,
                "Jwt:ExpiresInMinutes inválido.")
            .ValidateOnStart();

        services.AddSingleton<IEmissorTokens, JwtTokenGenerator>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((options, jwt) =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Value.Issuer,
                    ValidAudience = jwt.Value.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt.Value.SigningKey)),
                    NameClaimType = ClaimTypes.NameIdentifier,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });
        services
            .AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());
        return services;
    }
}
