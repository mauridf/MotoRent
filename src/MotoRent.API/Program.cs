using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MotoRent.API.Extensions;
using MotoRent.Infrastructure;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MotoRent.Infrastructure.Data;
using System.Diagnostics;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Configuração para substituir placeholders por variáveis de ambiente
string ReplacePlaceholders(string value)
{
    if (string.IsNullOrEmpty(value)) return value;

    return value
        .Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD"))
        .Replace("{JWT_SECRET_KEY}", Environment.GetEnvironmentVariable("JWT_SECRET_KEY"))
        .Replace("{MINIO_ENDPOINT}", Environment.GetEnvironmentVariable("MINIO_ENDPOINT"))
        .Replace("{MINIO_ACCESS_KEY}", Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY"))
        .Replace("{MINIO_SECRET_KEY}", Environment.GetEnvironmentVariable("MINIO_SECRET_KEY"));
}

// Verifica se está sendo executado pelo EF Core Tools
if (args.Any(x => x.Contains("ef", StringComparison.OrdinalIgnoreCase)))
{
    // Modo EF Core Tools - configuração mínima
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(ReplacePlaceholders(builder.Configuration.GetConnectionString("DefaultConnection"))));
}
else
{
    // Configuração normal para execução da API

    // Configuração do DbContext
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(ReplacePlaceholders(builder.Configuration.GetConnectionString("DefaultConnection"))));

    // Configuração da infraestrutura
    builder.Services.AddInfrastructure(builder.Configuration);

    // Configuração da autenticação JWT
    var jwtKey = ReplacePlaceholders(builder.Configuration["Jwt:Key"]);
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("JWT Key not configured");
    }

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

    // Configuração do MinIO (se necessário)
    var storageSettings = builder.Configuration.GetSection("StorageSettings");
    if (storageSettings["StorageType"] == "MinIO")
    {
        storageSettings["MinIO:Endpoint"] = ReplacePlaceholders(storageSettings["MinIO:Endpoint"]);
        storageSettings["MinIO:AccessKey"] = ReplacePlaceholders(storageSettings["MinIO:AccessKey"]);
        storageSettings["MinIO:SecretKey"] = ReplacePlaceholders(storageSettings["MinIO:SecretKey"]);
    }

    builder.Services.AddHostedService<MinioHealthCheckService>();

    // Inicia o MinIO automaticamente
    try
    {
        var minioUser = Environment.GetEnvironmentVariable("MINIO_ROOT_USER");
        var minioPassword = Environment.GetEnvironmentVariable("MINIO_ROOT_PASSWORD");
        var minioDataPath = Environment.GetEnvironmentVariable("MINIO_DATA_PATH") ?? "C:\\minio-data";

        if (string.IsNullOrEmpty(minioUser) || string.IsNullOrEmpty(minioPassword))
        {
            throw new InvalidOperationException("MinIO credentials not configured in environment variables");
        }

        var minioProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "minio.exe",
                Arguments = $@"server {minioDataPath} --console-address "":9001""",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Environment =
                {
                    ["MINIO_ROOT_USER"] = minioUser,
                    ["MINIO_ROOT_PASSWORD"] = minioPassword,
                    ["MINIO_BROWSER"] = "ON"
                }
            }
        };

        minioProcess.Start();
        minioProcess.OutputDataReceived += (sender, args) =>
            Console.WriteLine($"[MinIO] {args.Data}");
        minioProcess.ErrorDataReceived += (sender, args) =>
            Console.Error.WriteLine($"[MinIO Error] {args.Data}");

        minioProcess.BeginOutputReadLine();
        minioProcess.BeginErrorReadLine();
        await Task.Delay(2000);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao iniciar MinIO: {ex.Message}");
    }
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apenas configurar autenticação se não for comando EF Core
if (!args.Any(x => x.Contains("ef", StringComparison.OrdinalIgnoreCase)))
{
    app.UseAuthentication();
}

app.UseAuthorization();
app.MapControllers();

app.Run();