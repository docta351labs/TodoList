using Scalar.AspNetCore;
using TodoList.Application;
using TodoList.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Scalar API Reference
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }))
   .WithName("HealthCheck");

app.Run();

public partial class Program { }
