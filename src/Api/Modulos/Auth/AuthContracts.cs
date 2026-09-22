namespace GeradorCertificadosOnline.Api.Modulos.Auth;

public sealed record LoginRequest(string Email, string Senha);

public sealed record CadastroRequest(string Email, string Senha);
