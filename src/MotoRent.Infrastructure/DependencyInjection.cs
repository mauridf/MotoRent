using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoRent.Domain.Interfaces;
using MotoRent.Infrastructure.Data;
using MotoRent.Infrastructure.Repositories;
using MotoRent.Infrastructure.Services;

namespace MotoRent.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuração do DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            // Registro dos repositórios
            services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IEntregadorRepository, EntregadorRepository>();
            //services.AddScoped<IAtendenteRepository, AtendenteRepository>();
            //services.AddScoped<IFotoDocumentoRepository, FotoDocumentoRepository>();
            //services.AddScoped<IHabilitarEntregadorRepository, HabilitarEntregadorRepository>();
            //services.AddScoped<IMarcaRepository, MarcaRepository>();
            //services.AddScoped<IModeloRepository, ModeloRepository>();
            //services.AddScoped<IMotoRepository, MotoRepository>();
            //services.AddScoped<IFotoMotoRepository, FotoMotoRepository>();
            //services.AddScoped<IManutencaoRepository, ManutencaoRepository>();
            //services.AddScoped<ILocacaoRepository, LocacaoRepository>();
            //services.AddScoped<IReservaRepository, ReservaRepository>();

            // Registro dos serviços
            services.AddScoped<IImageUploadService, ImageUploadService>();
            //services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<IMotoService, MotoService>();
            //services.AddScoped<IEntregadorService, EntregadorService>();
            //services.AddScoped<ILocacaoService, LocacaoService>();

            // Configuração do UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}