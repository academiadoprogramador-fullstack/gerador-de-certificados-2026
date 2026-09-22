using GeradorCertificadosOnline.Dominio.Modulos.Cursos;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.DTOs;

public sealed record CursoDto(
    Guid Id,
    string Nome,
    int CargaHoraria,
    DateOnly DataConclusao)
{
    public static CursoDto From(Curso curso)
    {
        return new CursoDto(
            curso.Id,
            curso.Nome,
            curso.CargaHoraria,
            curso.DataConclusao);
    }
}
