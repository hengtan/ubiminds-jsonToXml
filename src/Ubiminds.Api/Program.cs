using Serilog;
using Ubiminds.Api.Extensions;
using Ubiminds.Api.Middlewares;
using Ubiminds.Infrastructure;
using Ubiminds.Infrastructure.Xml.Interfaces;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddSwaggerDocumentation(); // ← Swagger

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation(); // ← Swagger UI
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }