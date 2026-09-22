namespace GeradorCertificadosOnline.Dominio.Modulos.Certificados;

/// <summary>Gera o PDF do certificado de conclusão de um aluno.</summary>
public interface ICertificadoPdfGenerator
{
    byte[] Gerar(string nomeAluno, string nomeCurso, int cargaHoraria, DateOnly dataConclusao);
}
