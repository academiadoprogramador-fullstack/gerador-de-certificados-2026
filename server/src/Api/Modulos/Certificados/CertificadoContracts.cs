namespace GeradorCertificadosOnline.Api.Modulos.Certificados;

public sealed record AlunoRequest(string Nome);
public sealed record SolicitarCertificadosRequest(IReadOnlyList<AlunoRequest?>? Alunos);
