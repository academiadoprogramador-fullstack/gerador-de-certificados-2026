using GeradorCertificadosOnline.Dominio.Compartilhado;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;

namespace GeradorCertificadosOnline.Dominio.Modulos.Cursos;

public sealed class Curso
{
    public const int TamanhoMaximoNome = 200;

    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public int CargaHoraria { get; private set; }
    public DateOnly DataConclusao { get; private set; }
    public DateTimeOffset CriadoEm { get; private set; }

    public List<ProcessamentoCertificados> Processamentos { get; private set; } = [];

    private Curso() { }

    public Curso(string nome, int cargaHoraria, DateOnly dataConclusao)
    {
        Id = Guid.CreateVersion7();
        Nome = nome?.Trim() ?? string.Empty;
        CargaHoraria = cargaHoraria;
        DataConclusao = dataConclusao;
        CriadoEm = DateTimeOffset.UtcNow;
    }

    public IReadOnlyList<ErroValidacao> Validar()
    {
        List<ErroValidacao> erros = [];

        if (Nome.Length == 0)
            erros.Add(new("nome", "Nome é obrigatório."));

        else if (Nome.Length > TamanhoMaximoNome)
            erros.Add(new("nome", $"Nome deve ter no máximo {TamanhoMaximoNome} caracteres."));

        if (CargaHoraria <= 0)
            erros.Add(new("cargaHoraria", "Carga horária deve ser maior que zero."));

        if (DataConclusao == default)
            erros.Add(new("dataConclusao", "Data de conclusão é obrigatória."));
        return erros;
    }
}
