using System.Text.Json.Serialization;
using System.Text.Json;
using HealthChecks;

var builder = WebApplication.CreateBuilder(args);

if (!OperatingSystem.IsWindows())
{
    throw new PlatformNotSupportedException("Only windows because of directory health check.");
}

builder.Services.ConfigureHealthChecks(builder.Configuration);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHealthChecks();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();