using System.Text.Json.Serialization;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Serilog;
using TodoList.Api.Endpoints;
using TodoList.Api.Middleware;
using TodoList.Application;
using TodoList.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .CreateLogger();

builder.Host.UseSerilog();

// Configure System.Text.Json string enum conversion
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configure Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddConsoleExporter();
    });

// Configure Health Checks
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5433;Database=todolist;Username=postgres;Password=changeme";
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString);

// Configure CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure Middleware Pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseCors();
app.UseMiddleware<MockAuthMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "openapi/{documentName}.json";
    });

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("TodoList API - Scalar")
               .WithTheme(ScalarTheme.Purple)
               .WithOpenApiRoutePattern("/openapi/{documentName}.json");
    });
}

app.MapHealthChecks("/health");

// Map Endpoints
var apiV1 = app.MapGroup("/api/v1");
var todoListsGroup = apiV1.MapGroup("/todolists");
todoListsGroup.MapTodoListEndpoints();
todoListsGroup.MapTodoItemEndpoints();

app.Run();

public partial class Program { }
