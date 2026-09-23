using GeradorCertificadosOnline.Dominio.Modulos.Cursos;

namespace GeradorCertificadosOnline.Dominio.Modulos.Certificados;

/// <summary>
/// Agrega os certificados de um curso e controla o ciclo de vida de um lote de geração.
/// Um lote só é finalizado depois que todos os certificados têm resultado e o ZIP é registrado.
/// </summary>
public sealed class ProcessamentoCertificados
{
    public Guid Id { get; private set; }
    public Guid CursoId { get; private set; }
    public StatusProcessamento Status { get; private set; } = StatusProcessamento.Pendente;
    public DateTimeOffset IniciadoEm { get; private set; }
    public DateTimeOffset? ConcluidoEm { get; private set; }

    public Curso Curso { get; private set; } = null!;
    public List<Certificado> Certificados { get; private set; } = [];
    public string? CaminhoZip { get; private set; }

    public int TotalCertificados => Certificados.Count;
    public int Gerados => Certificados.Count(c => c.StatusGeracao == StatusGeracao.Gerado);
    public int Falhas => Certificados.Count(c => c.StatusGeracao == StatusGeracao.Falha);
    public bool TodosCertificadosProcessados => Gerados + Falhas == TotalCertificados;

    public bool EstaFinalizado => Status is
                StatusProcessamento.Concluido or
                StatusProcessamento.ConcluidoComFalhas;

    private ProcessamentoCertificados() { }

    public ProcessamentoCertificados(Guid cursoId, IReadOnlyList<Certificado> certificados)
    {
        ArgumentNullException.ThrowIfNull(certificados);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(certificados.Count);

        Id = Guid.CreateVersion7();
        CursoId = cursoId;

        foreach (var certificado in certificados)
        {
            certificado.VincularAo(this);
            Certificados.Add(certificado);
        }

        IniciadoEm = DateTimeOffset.UtcNow;
    }

    public void IniciarGeracao()
    {
        Status = StatusProcessamento.GerandoCertificados;
    }

    /// <summary>
    /// Registra o PDF de um certificado pendente. Retorna falso se o item já tiver resultado,
    /// permitindo que uma reentrega da mensagem seja idempotente.
    /// </summary>
    public bool RegistrarSucesso(Guid certificadoId, string caminhoArquivo)
    {
        var certificado = EncontrarPendente(certificadoId);

        if (certificado is null)
            return false;

        certificado.RegistrarGeracao(caminhoArquivo);

        return true;
    }

    /// <summary>
    /// Registra a falha de um certificado pendente sem interromper os demais itens do lote.
    /// </summary>
    public bool RegistrarFalha(Guid certificadoId)
    {
        var certificado = EncontrarPendente(certificadoId);

        if (certificado is null)
            return false;

        certificado.RegistrarFalha();

        return true;
    }

    /// <summary>
    /// Registra o ZIP e fecha o lote como concluído ou concluído com falhas.
    /// Todos os certificados precisam ter resultado antes desta operação.
    /// </summary>
    public void RegistrarZip(string caminhoZip)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caminhoZip);

        if (!TodosCertificadosProcessados)
            throw new InvalidOperationException("O processamento ainda possui certificados pendentes.");

        CaminhoZip = caminhoZip;
        Status = Falhas > 0
            ? StatusProcessamento.ConcluidoComFalhas
            : StatusProcessamento.Concluido;
        ConcluidoEm = DateTimeOffset.UtcNow;
    }

    private Certificado? EncontrarPendente(Guid certificadoId)
    {
        return Certificados.FirstOrDefault(c =>
            c.Id == certificadoId && c.StatusGeracao == StatusGeracao.Pendente);
    }
}
