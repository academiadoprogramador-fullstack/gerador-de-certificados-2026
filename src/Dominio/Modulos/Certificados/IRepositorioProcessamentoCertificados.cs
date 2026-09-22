namespace GeradorCertificadosOnline.Dominio.Modulos.Certificados;

public interface IRepositorioProcessamentoCertificados
{
    Task<Guid> CriarLoteAsync(Guid cursoId, IReadOnlyList<Certificado> certificados, CancellationToken cancellationToken);
    Task SalvarAsync(ProcessamentoCertificados processamento, CancellationToken cancellationToken);
    Task<ProcessamentoCertificados?> SelecionarPorCursoAsync(Guid cursoId, CancellationToken cancellationToken);
    Task<ProcessamentoCertificados?> SelecionarPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExisteEmAndamentoAsync(Guid cursoId, CancellationToken cancellationToken);
}
