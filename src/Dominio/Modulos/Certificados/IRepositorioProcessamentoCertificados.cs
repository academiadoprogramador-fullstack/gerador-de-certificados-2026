namespace GeradorCertificadosOnline.Dominio.Modulos.Certificados;

/// <summary>
/// Persiste o agregado de processamento e aplica as consultas usadas pelo fluxo de geração.
/// </summary>
public interface IRepositorioProcessamentoCertificados
{
    /// <summary>Cria e persiste um lote completo de forma atômica.</summary>
    Task<Guid> CriarLoteAsync(Guid cursoId, IReadOnlyList<Certificado> certificados, CancellationToken cancellationToken);

    /// <summary>Persiste o estado atual do agregado e de seus certificados.</summary>
    Task SalvarAsync(ProcessamentoCertificados processamento, CancellationToken cancellationToken);

    /// <summary>Obtém o processamento mais recente do curso, incluindo seus certificados.</summary>
    Task<ProcessamentoCertificados?> SelecionarPorCursoAsync(Guid cursoId, CancellationToken cancellationToken);

    /// <summary>Obtém um processamento pelo identificador, incluindo curso e certificados.</summary>
    Task<ProcessamentoCertificados?> SelecionarPorIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Indica se o curso possui lote pendente ou em geração.</summary>
    Task<bool> ExisteEmAndamentoAsync(Guid cursoId, CancellationToken cancellationToken);
}
