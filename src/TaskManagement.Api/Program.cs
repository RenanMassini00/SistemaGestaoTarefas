using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using TaskManagement.Api.Middleware;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;
using TaskManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var detalhes = new ValidationProblemDetails(context.ModelState)
        {
            Title = "Um ou mais dados informados são inválidos.",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Verifique os campos indicados e envie a requisição novamente.",
            Instance = context.HttpContext.Request.Path
        };

        detalhes.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        return new BadRequestObjectResult(detalhes);
    };
});

builder.Services.AddProblemDetails();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddInfrastructure();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Gestão de Tarefas",
        Version = "v1",
        Description = "API RESTful para cadastrar, consultar, buscar, atualizar e excluir tarefas.",
        Contact = new OpenApiContact
        {
            Name = "Equipe de Desenvolvimento"
        }
    });

    var arquivoXml = Path.Combine(
        AppContext.BaseDirectory,
        "TaskManagement.Api.xml");

    if (File.Exists(arquivoXml))
    {
        options.IncludeXmlComments(arquivoXml);
    }
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Gestão de Tarefas v1");
    options.DocumentTitle = "Documentação da API de Gestão de Tarefas";
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program;
