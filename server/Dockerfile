FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Api/GeradorCertificadosOnline.Api.csproj src/Api/
COPY src/Aplicacao/GeradorCertificadosOnline.Aplicacao.csproj src/Aplicacao/
COPY src/Dominio/GeradorCertificadosOnline.Dominio.csproj src/Dominio/
COPY src/Infraestrutura/GeradorCertificadosOnline.Infraestrutura.csproj src/Infraestrutura/
RUN dotnet restore src/Api/GeradorCertificadosOnline.Api.csproj

COPY . .
RUN dotnet publish src/Api/GeradorCertificadosOnline.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .
USER app

ENTRYPOINT ["dotnet", "GeradorCertificadosOnline.Api.dll"]
