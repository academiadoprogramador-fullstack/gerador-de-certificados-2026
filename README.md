# GeradorCertificadosOnline

Projeto didático de arquitetura orientada a eventos (ASP.NET Core + RabbitMQ + MassTransit) da Academia do Programador. A especificação técnica no formato de requisitos está em [.docs/especificacao-tecnica.md](.docs/especificacao-tecnica.md), com versão para importação no [Google Docs](.docs/especificacao-tecnica.docx), e a especificação detalhada de arquitetura está em [.docs/especificação.md](.docs/especificação.md).

## Estrutura

| Projeto                                 | Tipo                  | Responsabilidade                                                                |
| --------------------------------------- | --------------------- | ------------------------------------------------------------------------------- |
| `src/Api`                               | ASP.NET Core Web API  | Controllers, contratos HTTP, JWT, Problem Details e OpenAPI                     |
| `src/Aplicacao`                         | Biblioteca de classes | Commands/queries e handlers MediatR, DTOs e resultados FluentResults            |
| `src/Dominio`                           | Biblioteca de classes | Entidades, validações e contratos de persistência, autenticação e arquivos      |
| `src/Infraestrutura`                    | Biblioteca de classes | SQL Server/EF Core, migrations, repositórios, filesystem e integrações externas |
| `tests/GeradorCertificadosOnline.Tests` | Testes automatizados  | Integração HTTP, persistência relacional e consumers                            |

O padrão de organização segue: módulos por funcionalidade, controllers enxutos, handlers, um consumer e um repositório agregado. A API compõe a Aplicação e a Infraestrutura via DI. O domínio não depende de EF Core/ASP.NET e a aplicação não depende da infraestrutura.

Os repositórios expõem operações dos casos de uso, em vez de um CRUD genérico. Em particular, `Curso` pode possuir vários `ProcessamentoCertificados` finalizados, mas apenas um ativo por vez; cada processamento possui `Id` próprio, agrega seus certificados e é persistido por um único repositório. A mensageria usa somente o comando `GerarCertificados` e o `GerarCertificadosConsumer`.

## Pré-requisitos

- .NET SDK 10
- Visual Studio com o SQL Server LocalDB (componente do instalador do Visual Studio)
- Conta gratuita no [CloudAMQP](https://www.cloudamqp.com/)

O ambiente não usa Docker: o banco roda no LocalDB e o RabbitMQ fica no CloudAMQP.

## Configuração (uma vez por máquina)

1. Restaure as ferramentas locais (`dotnet-ef`):

   ```bash
   dotnet tool restore
   ```

2. Crie o banco no LocalDB aplicando as migrations:

   ```bash
    dotnet ef database update --project src/Infraestrutura --startup-project src/Api
   ```

3. Configure a chave de assinatura do JWT nos user-secrets. Gere uma chave aleatória com pelo menos 32 caracteres:

   ```bash
   dotnet user-secrets set "Jwt:SigningKey" "<chave-aleatoria-com-32+-caracteres>" --project src/Api
   ```

4. Crie uma instância gratuita no CloudAMQP, copie a **AMQP URL** (começa com `amqps://`) e guarde-a nos user-secrets:

   ```bash
    dotnet user-secrets set "ConnectionStrings:RabbitMq" "amqps://usuario:senha@host/vhost" --project src/Api
   ```

   A conexão com o RabbitMQ passa a ser usada na Fase 3.

## Como rodar

```bash
dotnet run --project src/Api
```

## Serviços locais

| Serviço             | Endereço                                                    |
| ------------------- | ----------------------------------------------------------- |
| Swagger UI          | http://localhost:5146/swagger                               |
| RabbitMQ Management | Painel da instância no CloudAMQP                            |
| SQL Server          | `(localdb)\MSSQLLocalDB`, banco `GeradorCertificadosOnline` |

## Autenticação

Todos os endpoints, exceto `POST /auth/cadastro` e `POST /auth/login`, exigem um JWT Bearer válido:

1. `POST /auth/cadastro` cria o usuário no ASP.NET Core Identity. A senha precisa ter pelo menos 8 caracteres, um dígito e um caractere não alfanumérico.
2. `POST /auth/login` valida o email e a senha no Identity e devolve `usuarioId`, `accessToken` e `dataExpiracaoEmUtc`.
3. No Swagger, clique em **Authorize** e informe `Bearer {token}`. Nos arquivos `.http` (veja `src/Api/GeradorCertificadosOnline.Api.http`), a requisição de login é nomeada (`@name login`) e as demais reaproveitam `{{login.response.body.$.accessToken}}`.

A autorização é uma política global; rotas públicas usam `AllowAnonymous` explicitamente. Em desenvolvimento, o documento OpenAPI e a interface Swagger são públicos. O JWT valida emissor, audiência, assinatura e expiração; `SigningKey` exige ao menos 32 bytes UTF-8 e `ExpiresInMinutes` deve ser positivo.

## Contrato HTTP

| Método | Rota                                      | Sucesso                              |
| ------ | ----------------------------------------- | ------------------------------------ |
| POST   | `/auth/cadastro`                          | 201, usuário criado                  |
| POST   | `/auth/login`                             | 200, usuário e token JWT             |
| POST   | `/cursos`                                 | 201, curso e `Location` consultável  |
| GET    | `/cursos/{cursoId}`                       | 200, curso                           |
| POST   | `/cursos/{cursoId}/certificados`          | 202, lote e `Location` para status   |
| GET    | `/cursos/{cursoId}/status`                | 200, processamento                   |
| GET    | `/cursos/{cursoId}/certificados`          | 200, certificados ordenados por nome |
| GET    | `/cursos/{cursoId}/certificados/download` | 200, `application/zip`               |

As rotas e os campos JSON existentes foram preservados. `GET /cursos/{cursoId}` permite seguir o `Location` do cadastro. Erros usam `application/problem+json` com `status`, `title`, `type` e `traceId`; validações incluem `errors` por campo. Falhas inesperadas retornam 500 com mensagem genérica e são registradas pelo pipeline de exceções.

O nome dos alunos acompanha o limite de 200 caracteres do banco. Entradas nulas e listas contendo alunos nulos retornam 400. Os usuários são persistidos pelo ASP.NET Core Identity.

## Certificados gerados

Ao solicitar a geração de certificados, a API envia o comando único `GerarCertificados`. O
`GerarCertificadosConsumer` carrega o agregado, processa cada aluno e gera os PDFs com [QuestPDF](https://www.questpdf.com/)
(licença Community, gratuita para uso educacional). Cada PDF é salvo em
`%LOCALAPPDATA%/GeradorCertificadosOnline/storage/{cursoId}/{certificadoId}.pdf` no Windows, em
`~/Library/Application Support/GeradorCertificadosOnline/storage/{cursoId}/{certificadoId}.pdf` no macOS e
em `~/.local/share/GeradorCertificadosOnline/storage/{cursoId}/{certificadoId}.pdf` no Linux. O caminho é
calculado por `Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)` e não precisa
ser configurado para rodar localmente.

O caminho é obtido em runtime usando `Environment.SpecialFolder.LocalApplicationData` e as pastas constantes
`GeradorCertificadosOnline/storage`. Isso evita gravar arquivos na pasta do projeto.

## Agregação e status

O `GerarCertificadosConsumer` registra cada resultado diretamente no agregado
`ProcessamentoCertificados`. Falhas na geração de um PDF são registradas como falhas individuais e não
interrompem os demais alunos. Falhas estruturais de persistência ou do ZIP propagam para o retry da
mensagem. Ao terminar, o mesmo consumer compacta os PDFs bem-sucedidos e registra o caminho no agregado.

Use `GET /cursos/{cursoId}/status` para acompanhar os contadores e `GET /cursos/{cursoId}/certificados`
para ver o status individual de cada aluno (RF-09).

## Zip e download

Depois de fechar os resultados, o storage compacta os PDFs já gerados do curso — apenas os certificados
com sucesso, mesmo quando o lote fecha como `ConcluidoComFalhas` (RN-06) — dentro do diretório LocalApplicationData
do usuário, em `GeradorCertificadosOnline/storage/{cursoId}/certificados-{processamentoId}.zip`
e grava o caminho em `ProcessamentoCertificados.CaminhoZip`. Diferente do caminho de cada PDF individual (interno
à API), e o caminho do zip é salvo absoluto.

`GET /cursos/{cursoId}/certificados/download` retorna o zip (`application/zip`) quando o status é
`Concluido` ou `ConcluidoComFalhas` e o zip já foi gravado (RN-03); enquanto isso não acontece, responde
`409 Conflict`. Acompanhe `GET /status` (campo `zipDisponivel`) até o zip ficar pronto antes de baixar.

## Resiliência

O consumer usa retry policy do MassTransit para falhas estruturais: 3 tentativas com intervalo incremental
(5s, 15s, 30s, seção 7 da especificação). Falhas individuais de PDF/storage são capturadas e registradas
no certificado para que o lote continue; falhas de persistência e compactação propagam para o retry.

Esgotadas as tentativas, o MassTransit publica `Fault<T>` automaticamente e move a mensagem para a fila
`{nome-da-fila}_error` (RN-07). Como o comando representa o lote inteiro, uma reentrega retoma somente os
certificados ainda pendentes; certificados gerados ou marcados como falha não são processados novamente.

O consumer verifica o status atual do lote e de cada certificado antes de processar (idempotência, RN-05).
Uma mensagem reentregue depois que o lote foi concluído é ignorada; uma reentrega durante o processamento
retoma os itens pendentes.

## Migrations

Para criar uma nova migration depois de alterar as entidades:

```bash
    dotnet ef migrations add NomeDaMigration --project src/Infraestrutura --startup-project src/Api --output-dir Compartilhado/Orm/Migrations
```

A migration inicial `20260915182222_Inicial` representa o modelo de certificados, incluindo a tabela
`ProcessamentosCertificados`. A migration `20260916135636_Identity` adiciona as tabelas do Identity. Como as
migrations anteriores foram recriadas, execute `dotnet ef database update` em um banco novo para aplicar o schema completo.
A migration `20260921135756_SimplificarProcessamentoCertificados` atualiza o índice de processamento ativo após a remoção do estado intermediário do ZIP.

## Verificação

```bash
dotnet restore GeradorCertificadosOnline.slnx
dotnet build GeradorCertificadosOnline.slnx
dotnet test GeradorCertificadosOnline.slnx
dotnet format GeradorCertificadosOnline.slnx --verify-no-changes
```

Os testes usam `WebApplicationFactory`, JWT real com configuração exclusiva de testes, SQLite relacional em memória e o Test Harness do MassTransit com transporte em memória. Cobrem validação, autorização, status HTTP, headers `Location`, mensagens enviadas, download, reinício de lote, falha individual e idempotência do agregado. Um teste compara o modelo SQL Server com o snapshot da migration sem se conectar a um servidor.

O teste do consumer executa a geração do lote, registro dos resultados e ZIP com filesystem temporário e substituto de PDF. A validação operacional de SQL Server, RabbitMQ e QuestPDF continua sendo feita com os serviços descritos acima e o arquivo `.http`.
