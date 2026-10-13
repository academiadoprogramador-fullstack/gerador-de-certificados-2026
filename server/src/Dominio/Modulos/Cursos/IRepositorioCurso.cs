namespace GeradorCertificadosOnline.Dominio.Modulos.Cursos;

public interface IRepositorioCurso
{
    Task CadastrarAsync(Curso curso, CancellationToken cancellationToken);
    Task<Curso?> SelecionarPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken);
}
