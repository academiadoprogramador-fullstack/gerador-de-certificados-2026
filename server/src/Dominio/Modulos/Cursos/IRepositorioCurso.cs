namespace GeradorCertificadosOnline.Dominio.Modulos.Cursos;

public interface IRepositorioCurso
{
    Task CadastrarAsync(Curso curso, CancellationToken cancellationToken);
    Task<Curso?> SelecionarPorIdAsync(Guid id, Guid usuarioId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Curso>> SelecionarTodosAsync(Guid usuarioId, CancellationToken cancellationToken);
    Task<bool> ExisteAsync(Guid id, Guid usuarioId, CancellationToken cancellationToken);
}
