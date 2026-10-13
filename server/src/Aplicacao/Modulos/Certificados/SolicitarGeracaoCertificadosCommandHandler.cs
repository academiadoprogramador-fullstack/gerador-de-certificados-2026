using FluentResults;
using GeradorCertificadosOnline.Aplicacao.Compartilhado;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.DTOs;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.Mensageria;
using GeradorCertificadosOnline.Aplicacao.Modulos.Certificados.Util;
using GeradorCertificadosOnline.Aplicacao.Modulos.Cursos.Util;
using GeradorCertificadosOnline.Dominio.Compartilhado;
using GeradorCertificadosOnline.Dominio.Modulos.Certificados;
using GeradorCertificadosOnline.Dominio.Modulos.Cursos;
using MassTransit;
using MediatR;

namespace GeradorCertificadosOnline.Aplicacao.Modulos.Certificados;

public sealed record AlunoCommand(string Nome);

public sealed record SolicitarGeracaoCertificadosCommand(
    Guid CursoId,
    IReadOnlyList<AlunoCommand?>? Alunos) : IRequest<Result<SolicitacaoCertificadosDto>>;

/// <summary>
/// Valida a solicitação, persiste o lote e envia o comando para geração assíncrona.
/// A persistência ocorre antes do envio ao broker; a V1 ainda não usa outbox transacional.
/// </summary>
public sealed class SolicitarGeracaoCertificadosCommandHandler(
    IRepositorioCurso repositorioCurso,
    IRepositorioProcessamentoCertificados repositorioProcessamento,
    ISendEndpointProvider endpointProvider)
    : IRequestHandler<SolicitarGeracaoCertificadosCommand, Result<SolicitacaoCertificadosDto>>
{
    public async Task<Result<SolicitacaoCertificadosDto>> Handle(
        SolicitarGeracaoCertificadosCommand command,
        CancellationToken cancellationToken)
    {
        List<ErroValidacao> erros = [];
        List<Certificado> certificados = [];

        if (command.Alunos is not { Count: > 0 })
            erros.Add(new("alunos", "A lista de alunos deve ter ao menos 1 aluno."));

        else
        {
            for (var i = 0; i < command.Alunos.Count; i++)
            {
                var aluno = command.Alunos[i];

                if (aluno is null)
                {
                    erros.Add(new($"alunos[{i}]", "Aluno é obrigatório."));
                    continue;
                }

                var certificado = new Certificado(aluno.Nome);

                var errosCertificado = certificado.Validar().Select(e => new ErroValidacao(
                    $"alunos[{i}].{e.Campo}",
                    e.Mensagem
                ));

                erros.AddRange(errosCertificado);
                certificados.Add(certificado);
            }
        }

        if (erros.Count > 0)
            return Result.Fail(Erros.Validacao(erros));

        if (!await repositorioCurso.ExisteAsync(command.CursoId, cancellationToken))
            return Result.Fail(ErrosDeCurso.NaoEncontrado());

        var existeEmAndamento = await repositorioProcessamento.ExisteEmAndamentoAsync(
            command.CursoId,
            cancellationToken);

        if (existeEmAndamento)
            return Result.Fail(ErrosDeCertificado.EmAndamento());
        try
        {
            var processamentoId = await repositorioProcessamento.CriarLoteAsync(
                command.CursoId,
                certificados,
                cancellationToken);

            var endpoint = await endpointProvider.GetSendEndpoint(
                new Uri("queue:gerar-certificados"));

            await endpoint.Send(
                new GerarCertificados(command.CursoId, processamentoId),
                cancellationToken);
        }
        catch (ConflitoDePersistenciaException)
        {
            return Result.Fail(ErrosDeCertificado.EmAndamento());
        }

        return Result.Ok(new SolicitacaoCertificadosDto(
            command.CursoId,
            certificados.Count,
            StatusProcessamento.Pendente.ToString()
        ));
    }
}
