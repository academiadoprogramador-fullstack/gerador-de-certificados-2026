namespace GeradorCertificadosOnline.Api.Compartilhado.Auth;

/// <summary>
/// Configuração de emissão/validação do JWT.
/// Issuer, Audience e ExpiresInMinutes vêm do appsettings; SigningKey vem de user-secrets
/// (chave "Jwt:SigningKey"), nunca do repositório.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string SigningKey { get; set; }
    public int ExpiresInMinutes { get; set; } = 60;
}
