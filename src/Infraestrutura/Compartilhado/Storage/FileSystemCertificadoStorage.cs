using System.IO.Compression;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;

namespace GeradorCertificadosOnline.Infraestrutura.Compartilhado.Storage;

/// <summary>
/// Grava os arquivos em "{LocalApplicationData}/GeradorCertificadosOnline/storage/{cursoId}".
/// </summary>
public sealed class FileSystemCertificadoStorage : ICertificadoStorage
{
    private const string ApplicationDirectoryName = "GeradorCertificadosOnline";
    private const string StorageDirectoryName = "storage";

    public async Task<string> SalvarAsync(
        Guid cursoId,
        Guid certificadoId,
        byte[] conteudoPdf,
        CancellationToken cancellationToken)
    {
        var pastaCurso = ObterPastaCurso(cursoId);
        var caminhoArquivo = Path.Combine(pastaCurso, $"{certificadoId}.pdf");

        await SalvarBytesAsync(caminhoArquivo, conteudoPdf, cancellationToken);

        return caminhoArquivo;
    }

    public async Task<string> CompactarAsync(
        Guid cursoId,
        Guid processamentoId,
        IReadOnlyList<string> caminhosPdf,
        CancellationToken cancellationToken)
    {
        var pastaCurso = ObterPastaCurso(cursoId);

        // O caminho absoluto é persistido para que a API possa abrir o arquivo depois.
        var caminhoArquivo = Path.GetFullPath(
            Path.Combine(pastaCurso, $"certificados-{processamentoId}.zip"));

        await using var stream = new MemoryStream();

        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var caminho in caminhosPdf)
                archive.CreateEntryFromFile(caminho, Path.GetFileName(caminho));
        }

        await SalvarBytesAsync(caminhoArquivo, stream.ToArray(), cancellationToken);

        return caminhoArquivo;
    }

    public Stream AbrirLeitura(string caminho)
    {
        return File.OpenRead(caminho);
    }

    private string ObterPastaCurso(Guid cursoId)
    {
        var pastaCurso = Path.Combine(ObterCaminhoPadrao(), cursoId.ToString());
        Directory.CreateDirectory(pastaCurso);

        return pastaCurso;
    }

    private static string ObterCaminhoPadrao()
    {
        var localApplicationData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData,
            Environment.SpecialFolderOption.Create);

        if (string.IsNullOrWhiteSpace(localApplicationData))
            throw new InvalidOperationException(
                "Não foi possível determinar o diretório LocalApplicationData do usuário.");

        return Path.Combine(
            localApplicationData,
            ApplicationDirectoryName,
            StorageDirectoryName);
    }

    private static Task SalvarBytesAsync(
        string caminhoArquivo,
        byte[] conteudo,
        CancellationToken cancellationToken)
    {
        return File.WriteAllBytesAsync(caminhoArquivo, conteudo, cancellationToken);
    }
}
