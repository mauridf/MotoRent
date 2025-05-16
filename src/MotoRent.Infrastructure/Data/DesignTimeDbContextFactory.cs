using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using MotoRent.Infrastructure.Data;
using System.Reflection;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // 1. Carregar variáveis de ambiente da raiz da solução
        var solutionPath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "../../../../../"));

        var envPath = Path.Combine(solutionPath, ".env");
        Console.WriteLine($"Buscando .env em: {envPath}");

        if (File.Exists(envPath))
        {
            Console.WriteLine(".env encontrado. Carregando variáveis...");
            DotNetEnv.Env.Load(envPath);
        }
        else
        {
            Console.WriteLine("AVISO: Arquivo .env não encontrado!");
        }

        // 2. Obter senha (com fallback para desenvolvimento)
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "Mt190720@";
        Console.WriteLine($"Usando senha: {(string.IsNullOrEmpty(dbPassword) ? "VAZIA" : " * *****")}");

        // 3. Construir connection string diretamente
        var connectionString = $"Server=localhost;Port=5432;Database=MotoRentDB;User Id=postgres;Password={dbPassword};";
        Console.WriteLine($"String de conexão final: {connectionString}");

        // 4. Criar e retornar o DbContext
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}