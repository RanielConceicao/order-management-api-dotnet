using OrderManagement.Application;
using OrderManagement.Infrastructure;
using OrderManagement.API.Middleware;
using Serilog;
using System.IO;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Ensure logs directory exists so Serilog can write rolling files
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "logs"));

// Configure Serilog from configuration (appsettings.json)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Management API", Version = "v1" });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure middleware and endpoints in a few small helper functions to keep Program.cs tidy
// Swagger will be enabled only in Development and will be served at the application root (/)
if (app.Environment.IsDevelopment())
{
    ConfigureSwagger(app);
}

app.UseSimpleExceptionMiddleware();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting web host");

    // Start the server and wait for shutdown
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// Local helper: configure swagger middleware
void ConfigureSwagger(WebApplication webApp)
{
    webApp.UseSwagger();
    webApp.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management API v1");
        options.RoutePrefix = string.Empty; // UI disponível na raiz '/'
    });
}