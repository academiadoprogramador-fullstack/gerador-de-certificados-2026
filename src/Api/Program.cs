using GeradorCertificadosOnline.Api.Compartilhado.Auth;
using GeradorCertificadosOnline.Api.Compartilhado.Http;
using GeradorCertificadosOnline.Aplicacao;
using GeradorCertificadosOnline.Infraestrutura;
using GeradorCertificadosOnline.Infraestrutura.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddJwtAuthServices();
builder.Services.AddHttpServices();
builder.Services.AddOpenApiServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "GeradorCertificadosOnline.Api v1"));

    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<CertificadosDbContext>();

    if (dbContext.Database.IsSqlServer())
        dbContext.Database.Migrate();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
