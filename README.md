# Certifica+

<p align="center">
  <strong>Reconheça a formação dos seus alunos.</strong><br />
  Organize os cursos da sua instituição, emita certificados e acompanhe cada etapa em um só lugar.
</p>

<p align="center">
  <a href="https://skillicons.dev">
    <img src="https://skillicons.dev/icons?i=angular,ts,tailwind,dotnet,cs,rabbitmq,azure,githubactions&perline=8" alt="Angular, TypeScript, Tailwind CSS, .NET, C#, RabbitMQ, Azure e GitHub Actions" />
  </a>
</p>

> **Certifica+** é uma aplicação full-stack para instituições de ensino organizarem seus cursos e emitirem certificados digitais para os alunos. A API processa as solicitações de forma assíncrona, gera certificados em PDF e disponibiliza um arquivo ZIP quando o lote estiver pronto.
>
> O projeto está em evolução: nesta etapa, o client oferece as telas de cadastro e login; a API já disponibiliza operações de cursos, solicitações de certificados, acompanhamento e download.

Desenvolvido durante o curso Fullstack da [Academia do Programador 2026](https://www.academiadoprogramador.net).

## O produto

- Cadastre e consulte os cursos oferecidos pela instituição.
- Solicite a emissão de certificados para os alunos de cada curso.
- Acompanhe o processamento e o resultado de cada certificado emitido.
- Baixe os certificados dos alunos em PDF, reunidos em um arquivo ZIP.
- Crie um acesso para a instituição; cursos e certificados ficam associados à conta autenticada.

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

### Executar a API em um container

É necessário ter o Docker instalado, acesso a um SQL Server e a um RabbitMQ acessíveis pelo container e uma chave de assinatura JWT com pelo menos 32 bytes. O banco precisa estar criado e com as migrations aplicadas.

Se a imagem do GHCR for privada, autentique-se antes do `pull` usando um token do GitHub com permissão `read:packages`:

```bash
echo "$GHCR_TOKEN" | docker login ghcr.io --username SEU_USUARIO_GITHUB --password-stdin
```

Crie um arquivo de ambiente fora do repositório, por exemplo `/caminho/seguro/api.env`, com os endereços e segredos do seu ambiente:

```dotenv
ConnectionStrings__SqlServer=Server=sql.exemplo.net,1433;Database=GeradorDeCertificadosDb;User Id=usuario;Password=senha;TrustServerCertificate=True
ConnectionStrings__RabbitMq=amqp://usuario:senha@rabbitmq.exemplo.net:5672
Jwt__SigningKey=troque-por-uma-chave-aleatoria-com-pelo-menos-32-bytes
Cors__AllowedOrigins__0=https://seu-client.exemplo.com
```

Restrinja o acesso ao arquivo e crie uma pasta persistente para os PDFs e ZIPs. A imagem executa como o usuário `app` (UID 1654):

```bash
chmod 600 /caminho/seguro/api.env
mkdir -p ./certificados-storage
sudo chown 1654:1654 ./certificados-storage
```

Baixe e execute a imagem, substituindo `latest` por uma tag `sha-...` se quiser fixar uma versão específica:

```bash
docker pull ghcr.io/academiadoprogramador-fullstack/gerador-de-certificados-2026:latest
docker run -d \
  --name certificados-api \
  --restart unless-stopped \
  --publish 8080:8080 \
  --env-file /caminho/seguro/api.env \
  --mount type=bind,source="$(pwd)/certificados-storage",target=/home/app/.local/share/GeradorCertificadosOnline/storage \
  ghcr.io/academiadoprogramador-fullstack/gerador-de-certificados-2026:latest
```

A API ficará disponível em `http://localhost:8080` e o Swagger em [`http://localhost:8080/swagger`](http://localhost:8080/swagger). Os nomes de variáveis usam `__` no lugar de `:` na configuração .NET. Não use `localhost` nas connection strings para apontar a serviços que rodam fora do container: nesse caso, informe um hostname acessível pela rede Docker. Não grave o arquivo de ambiente no repositório nem passe segredos diretamente na linha de comando.

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

As rotas protegidas aceitam autenticação **Bearer**. Cursos e certificados são associados ao usuário do token; operações sobre recursos de outros usuários não os expõem. Os erros da API seguem `ProblemDetails`; erros de validação incluem mensagens por campo. Exemplos de requisições estão em `server/src/Api/GeradorCertificadosOnline.Api.http`.

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
