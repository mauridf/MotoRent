using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MotoRent.API.Extensions;
using MotoRent.Infrastructure;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MotoRent.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

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

// Configuração do DbContext
var connectionString = ReplacePlaceholders(builder.Configuration.GetConnectionString("DefaultConnection"));
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();