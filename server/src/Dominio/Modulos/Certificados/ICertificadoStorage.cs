namespace GeradorCertificadosOnline.Dominio.Modulos.Certificados;

/// <summary>
/// Abstrai a gravação e leitura dos arquivos de certificados.
/// </summary>
public interface ICertificadoStorage
{
    /// <summary>Grava o PDF e retorna o caminho onde foi salvo.</summary>
    Task<string> SalvarAsync(Guid cursoId, Guid certificadoId, byte[] conteudoPdf, CancellationToken cancellationToken);

    Task<string> CompactarAsync(
        Guid cursoId,
        Guid processamentoId,
        IReadOnlyList<string> caminhosPdf,
        CancellationToken cancellationToken);

    Stream AbrirLeitura(string caminho);
}
