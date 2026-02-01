using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ProjetoFinal.Application.Abstractions.Repositories;
using ProjetoFinal.Application.Services;
using ProjetoFinal.Infrastructure.Entities;
using ProjetoFinal.Infrastructure.Persistence;
using ProjetoFinal.Infrastructure.Repositories;
using ProjetoFinal.Infrastructure.Services;

namespace ProjetoFinal.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // Connection string
            var conn = config.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' não configurada.");

            // DbContext
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(conn));

            // Identity
            services
                .AddIdentityCore<ApplicationUser>(o =>
                {
                    o.User.RequireUniqueEmail = true;
                    o.Password.RequiredLength = 6;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireDigit = false;
                })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>();

            // Repositórios (Infra → Application Abstractions)
            services.AddScoped<ICursoRepository, EfCursoRepository>();
            services.AddScoped<IMateriaRepository, EfMateriaRepository>();
            services.AddScoped<ITipoCursoRepository, EfTipoCursoRepository>();

            // Serviços de aplicação
            services.AddScoped<ICursoQueryService, CursoQueryService>();
            services.AddScoped<ICursoLookupService, CursoLookupService>();

            return services;
        }
    }
}
