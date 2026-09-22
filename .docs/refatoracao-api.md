# Refatoração da API

Referência: `delivery-app-2026@v8`, commit `0c084dfb0c0af632e62891fcd2eb7b44a6e5c7bf`.

## Fronteiras

- **API:** requests, controllers, mapeamento de `Result` para HTTP e geração JWT. Os controllers retornam diretamente os DTOs da Aplicação; nenhum controller acessa banco, broker ou filesystem diretamente.
- **Aplicação:** handlers MediatR que coordenam domínio, o repositório agregado e o envio do comando `GerarCertificados`. As respostas são DTOs, sem tipos ASP.NET.
- **Domínio:** entidades com setters privados, normalização/validação e contratos especializados. `Curso` mantém o histórico de `ProcessamentosCertificados`, cada um com `Id` próprio e sua lista de certificados.
- **Mensageria na Aplicação:** o contrato `GerarCertificados`, o `GerarCertificadosConsumer` e a configuração do MassTransit/RabbitMQ ficam no módulo de certificados.
- **Infraestrutura:** implementação dos repositórios, storage e geração de PDF consumidos pelas portas do Domínio.

## Persistência

`ProcessamentoCertificados` possui `Id` próprio e `Certificado` referencia o processamento por `ProcessamentoId`. Um curso pode ter vários processamentos finalizados; um índice único filtrado impede mais de um processamento ativo para o mesmo curso. As mensagens carregam `ProcessamentoId` para que lotes históricos não atualizem o processamento mais recente.

A criação do lote tem um único `SaveChangesAsync`, cadastrando o processamento e seus certificados na mesma unidade de persistência. O agregado é recarregado com o curso e os certificados para que uma reentrega possa continuar os itens pendentes. Violações de unicidade SQL Server são convertidas em conflito da aplicação; outras falhas de persistência seguem para o pipeline de retry.

O agregado é carregado com seus certificados, atualizado após cada resultado e salvo pelo mesmo repositório. O storage também compacta e abre o ZIP, removendo uma porta específica de arquivos. A refatoração não adiciona outbox ou saga.

## Compatibilidade HTTP

- Mantidas as rotas sem prefixo `/api`, nomes das operações OpenAPI, campos JSON, enums como strings e datas.
- Acrescentada a consulta de curso por ID para atender ao `Location` do cadastro.
- Erros padronizados em Problem Details; respostas de erro anteriormente vazias ou anônimas agora têm corpo estruturado.
- Validação também rejeita alunos nulos e nomes maiores que a coluna do banco.
- Cadastro ocorre em `POST /auth/cadastro`; login em `POST /auth/login` retorna `usuarioId`, `accessToken` e `dataExpiracaoEmUtc`, com credenciais inválidas respondendo 401.
- Download usa stream aberto pela infraestrutura, cuja liberação é feita pelo executor de arquivo do ASP.NET.

## Mensageria

`POST /cursos/{cursoId}/certificados` persiste o lote e envia `GerarCertificados`. A API hospeda um único consumer
para geração individual, agregação e ZIP. O comando usa `Send` e o retry estrutural mantém os intervalos de 5s, 15s
e 30s. A reentrega retoma apenas certificados pendentes e ignora lotes concluídos.

## Verificações automatizadas

Ver `tests/GeradorCertificadosOnline.Tests` e os comandos no README. Os testes isolam configurações e não acessam credenciais ou serviços externos. A referência a SQL Server no teste de snapshot serve apenas para construir o modelo relacional.
