namespace GeradorCertificadosOnline.Api.Modulos.Cursos;

public sealed record CriarCursoRequest(
    string Nome,
    int CargaHoraria,
    DateOnly DataConclusao);
