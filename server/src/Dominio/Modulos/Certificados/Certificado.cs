using GeradorCertificadosOnline.Dominio.Compartilhado;

namespace GeradorCertificadosOnline.Dominio.Modulos.Certificados;

public sealed class Certificado
{
    public const int TamanhoMaximoNome = 200;
    public Guid Id { get; private set; }
    public Guid ProcessamentoId { get; private set; }
    public string NomeAluno { get; private set; } = string.Empty;
    public StatusGeracao StatusGeracao { get; private set; } = StatusGeracao.Pendente;

    public string? CaminhoArquivo { get; private set; }
    public DateTimeOffset? GeradoEm { get; private set; }

    public ProcessamentoCertificados Processamento { get; private set; } = null!;

    private Certificado() { }

    public Certificado(string nomeAluno)
    {
        Id = Guid.CreateVersion7();
        NomeAluno = nomeAluno?.Trim() ?? string.Empty;
    }

    public void VincularAo(ProcessamentoCertificados processamento)
    {
        ArgumentNullException.ThrowIfNull(processamento);

        Processamento = processamento;
        ProcessamentoId = processamento.Id;
    }

    public IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (NomeAluno.Length == 0)
            erros.Add(new("nome", "Nome é obrigatório."));

        else if (NomeAluno.Length > TamanhoMaximoNome)
            erros.Add(new("nome", $"Nome deve ter no máximo {TamanhoMaximoNome} caracteres."));

        return erros;
    }

    public void RegistrarGeracao(string caminhoArquivo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caminhoArquivo);

        StatusGeracao = StatusGeracao.Gerado;
        CaminhoArquivo = caminhoArquivo;
        GeradoEm = DateTimeOffset.UtcNow;
    }

    public void RegistrarFalha()
    {
        StatusGeracao = StatusGeracao.Falha;
    }
}
