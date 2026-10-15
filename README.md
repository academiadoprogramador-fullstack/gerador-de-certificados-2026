# Certifica+

<p align="center">
  <strong>Aprendizado que vira conquista.</strong><br />
  Organize cursos, gere certificados e acompanhe cada etapa em um só lugar.
</p>

<p align="center">
  <a href="https://skillicons.dev">
    <img src="https://skillicons.dev/icons?i=angular,ts,tailwind,dotnet,cs,rabbitmq,azure,githubactions&perline=8" alt="Angular, TypeScript, Tailwind CSS, .NET, C#, RabbitMQ, Azure e GitHub Actions" />
  </a>
</p>

> **Certifica+** é uma aplicação full-stack para administrar cursos e emitir certificados digitais. A API processa as solicitações de forma assíncrona, gera certificados em PDF e disponibiliza um arquivo ZIP quando o lote estiver pronto.
>
> O projeto está em evolução: nesta etapa, o client oferece as telas de cadastro e login; a API já disponibiliza operações de cursos, solicitações de certificados, acompanhamento e download.

Desenvolvido durante o curso Fullstack da [Academia do Programador 2026](https://www.academiadoprogramador.net).

## O produto

- Crie e consulte cursos.
- Solicite certificados para uma lista de alunos.
- Acompanhe o progresso de cada processamento e o resultado de cada certificado.
- Baixe os certificados gerados em PDF, reunidos em um ZIP.
- Crie uma conta e autentique-se para acessar as operações protegidas da API.

O processamento é orientado a eventos: a API publica uma solicitação no RabbitMQ e um consumer gera os PDFs em segundo plano. Falhas individuais são registradas sem interromper o restante do lote; o estado e a disponibilidade do ZIP podem ser consultados pela API.

## Tecnologias

| Área                   | Tecnologias                                                      |
| ---------------------- | ---------------------------------------------------------------- |
| Client                 | Angular, TypeScript, Tailwind CSS, Signal Forms e Lucide Angular |
| API                    | ASP.NET Core, C# e .NET 10                                       |
| Identidade e segurança | ASP.NET Core Identity e JWT Bearer                               |
| Persistência           | SQL Server e Entity Framework Core                               |
| Mensageria             | RabbitMQ e MassTransit                                           |
| Documentos             | QuestPDF e geração de arquivos ZIP                               |
| Entrega                | GitHub Actions e Azure App Service                               |

## Organização do projeto

```text
client/                         Aplicação Angular
server/
├── src/
│   ├── Api/                    Endpoints HTTP, autenticação e OpenAPI
│   ├── Aplicacao/               Casos de uso e handlers
│   ├── Dominio/                 Entidades e contratos do domínio
│   └── Infraestrutura/          SQL Server, migrations e integrações
├── GeradorCertificadosOnline.slnx
└── dotnet-tools.json
```

A API é dividida em camadas: o domínio não depende de ASP.NET Core ou Entity Framework, e a aplicação define os casos de uso sem depender da infraestrutura. A API conecta essas camadas por injeção de dependência.

## Executar localmente

### Pré-requisitos

- .NET SDK 10
- Node.js compatível com Angular 22 e npm
- SQL Server acessível pela connection string de desenvolvimento
- RabbitMQ acessível pela connection string de desenvolvimento

As connection strings locais ficam em `server/src/Api/appsettings.Development.json`. Para outros ambientes, configure os valores usando variáveis de ambiente ou User Secrets; não grave credenciais reais no repositório.

### Preparar a API

Na raiz do repositório, restaure a ferramenta do Entity Framework:

```bash
dotnet tool restore --tool-manifest server/dotnet-tools.json
```

Configure a chave de assinatura JWT e as connection strings para os serviços disponíveis no seu ambiente:

```bash
dotnet user-secrets set "Jwt:SigningKey" "<chave-aleatoria-com-pelo-menos-32-caracteres>" --project server/src/Api/GeradorCertificadosOnline.Api.csproj
dotnet user-secrets set "ConnectionStrings:SqlServer" "<connection-string-sql-server>" --project server/src/Api/GeradorCertificadosOnline.Api.csproj
dotnet user-secrets set "ConnectionStrings:RabbitMq" "<connection-string-rabbitmq>" --project server/src/Api/GeradorCertificadosOnline.Api.csproj
```

Crie ou atualize o banco de dados aplicando as migrations:

```bash
dotnet ef database update \
  --project server/src/Infraestrutura/GeradorCertificadosOnline.Infraestrutura.csproj \
  --startup-project server/src/Api/GeradorCertificadosOnline.Api.csproj
```

Inicie a API:

```bash
dotnet run --project server/src/Api/GeradorCertificadosOnline.Api.csproj
```

A API local fica disponível em `http://localhost:5146`; o Swagger fica em [`http://localhost:5146/swagger`](http://localhost:5146/swagger).

### Preparar o client

Em outro terminal, na raiz do repositório:

```bash
cd client
npm ci
npm start
```

O client fica disponível em [`http://localhost:4200`](http://localhost:4200). Em desenvolvimento, essa origem já está permitida pela política CORS da API.

## Autenticação e CORS

O cadastro usa `POST /auth/cadastro`; o login usa `POST /auth/login`. O cadastro requer senha com ao menos 8 caracteres, um número e um caractere especial. O login retorna um token JWT; chamadas às operações protegidas devem enviá-lo como Bearer. A integração do client às operações de cursos ainda será desenvolvida.

Em produção, configure a origem pública do client na API com `Cors__AllowedOrigins__0`, por exemplo:

```text
Cors__AllowedOrigins__0=https://seu-client.exemplo.com
```

Para permitir mais de uma origem, use índices adicionais, como `Cors__AllowedOrigins__1`. Informe apenas a origem — sem caminho ou barra final. O client de produção aponta inicialmente para `https://gerador-de-certificados-fullstack.azurewebsites.net`; ajuste `client/src/environments/environment.production.ts` se a URL da API mudar.

## API

| Método | Rota                                      | Resultado                             |
| ------ | ----------------------------------------- | ------------------------------------- |
| `POST` | `/auth/cadastro`                          | Cria um usuário                       |
| `POST` | `/auth/login`                             | Autentica e retorna um JWT            |
| `POST` | `/cursos`                                 | Cria um curso                         |
| `GET`  | `/cursos`                                 | Lista cursos ordenados por nome       |
| `GET`  | `/cursos/{cursoId}`                       | Consulta um curso                     |
| `POST` | `/cursos/{cursoId}/certificados`          | Inicia a geração de certificados      |
| `GET`  | `/cursos/{cursoId}/status`                | Consulta o progresso do processamento |
| `GET`  | `/cursos/{cursoId}/certificados`          | Lista certificados e seus estados     |
| `GET`  | `/cursos/{cursoId}/certificados/download` | Baixa o ZIP gerado                    |

As rotas protegidas aceitam autenticação **Bearer**. Os erros da API seguem `ProblemDetails`; erros de validação incluem mensagens por campo. Exemplos de requisições estão em `server/src/Api/GeradorCertificadosOnline.Api.http`.

## Processamento de certificados

O processamento usa MassTransit com RabbitMQ e gera os PDFs com [QuestPDF](https://www.questpdf.com/). Os arquivos são armazenados no diretório local de dados da aplicação. Quando o lote termina, os certificados gerados com sucesso são reunidos em um ZIP; o download é liberado quando o arquivo estiver disponível.

O consumer retoma apenas certificados pendentes após uma reentrega da mensagem. Falhas individuais são reportadas no estado do certificado, enquanto falhas estruturais podem acionar as retentativas configuradas no MassTransit.

## Verificações

Build da solução .NET:

```bash
dotnet build server/GeradorCertificadosOnline.slnx --configuration Release
```

Testes e build do client:

```bash
npm --prefix client test -- --watch=false
npm --prefix client run build
```

## Deploy

O workflow **Deploy .NET API to Azure App Service** publica a API e executa as migrations quando há alterações em `server/**`. Também pode ser iniciado manualmente pelo GitHub Actions. A configuração de deploy usa secrets do repositório para connection strings e chaves de serviço.
