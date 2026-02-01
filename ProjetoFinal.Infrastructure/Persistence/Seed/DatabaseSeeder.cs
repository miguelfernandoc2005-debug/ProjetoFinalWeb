using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjetoFinal.Domain.Entities;
using ProjetoFinal.Infrastructure.Entities;

namespace ProjetoFinal.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// Aplica migrations e insere dados iniciais de forma idempotente.
    /// Ordem: Migrate -> TiposUsuario -> Roles -> Usuários padrão -> Catálogos (Materias, TiposCurso, Cursos)
    /// </summary>
    public static class DatabaseSeeder
    {
        public static async Task EnsureSeededAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;

            var ctx = sp.GetRequiredService<AppDbContext>();
            await ctx.Database.MigrateAsync();

            // =======================
            // Tipos de usuário
            // =======================
            if (!await ctx.TiposUsuario.AnyAsync())
            {
                ctx.TiposUsuario.AddRange(
                    new TipoUsuario { Nome = "Administrador" },
                    new TipoUsuario { Nome = "Docente" },
                    new TipoUsuario { Nome = "Aluno" }
                );
                await ctx.SaveChangesAsync();
            }

            var tipoAdminId = await ctx.TiposUsuario.Where(t => t.Nome == "Administrador").Select(t => t.Id).FirstAsync();
            var tipoDocenteId = await ctx.TiposUsuario.Where(t => t.Nome == "Docente").Select(t => t.Id).FirstAsync();
            var tipoAlunoId = await ctx.TiposUsuario.Where(t => t.Nome == "Aluno").Select(t => t.Id).FirstAsync();

            // =======================
            // Roles do Identity
            // =======================
            var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            async Task EnsureRoleAsync(string roleName)
            {
                if (!await roleMgr.RoleExistsAsync(roleName))
                {
                    var create = await roleMgr.CreateAsync(new IdentityRole<Guid>(roleName));
                    if (!create.Succeeded)
                        throw new InvalidOperationException($"Falha ao criar role '{roleName}': {string.Join("; ", create.Errors.Select(e => e.Description))}");
                }
            }

            await EnsureRoleAsync("Admin");
            await EnsureRoleAsync("Docente");
            await EnsureRoleAsync("Aluno");

            // =======================
            // Usuários padrão
            // =======================
            var userMgr = sp.GetRequiredService<UserManager<ApplicationUser>>();

            async Task EnsureUserAsync(string userName, string email, string password, string role, int idTipoUsuario)
            {
                var user = await userMgr.FindByEmailAsync(email);
                if (user is null)
                {
                    user = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = userName,
                        Email = email,
                        EmailConfirmed = true,
                        IsActive = true,
                        IdTipoUsuario = idTipoUsuario,
                        CreatedAt = DateTime.UtcNow
                    };

                    var create = await userMgr.CreateAsync(user, password);
                    if (!create.Succeeded)
                        throw new InvalidOperationException($"Falha ao criar usuário '{email}': {string.Join("; ", create.Errors.Select(e => e.Description))}");
                }
                else
                {
                    if (!user.IsActive || user.IdTipoUsuario != idTipoUsuario)
                    {
                        user.IsActive = true;
                        user.IdTipoUsuario = idTipoUsuario;
                        user.UpdatedAt = DateTime.UtcNow;
                        await ctx.SaveChangesAsync();
                    }
                }

                if (!await userMgr.IsInRoleAsync(user, role))
                {
                    var add = await userMgr.AddToRoleAsync(user, role);
                    if (!add.Succeeded)
                        throw new InvalidOperationException($"Falha ao atribuir role '{role}' ao usuário '{email}': {string.Join("; ", add.Errors.Select(e => e.Description))}");
                }
            }

            await EnsureUserAsync("Admin", "admin@projetofinal.local", "Admin@123", "Admin", tipoAdminId);
            await EnsureUserAsync("Docente", "professor@projetofinal.local", "Docente@123", "Docente", tipoDocenteId);
            await EnsureUserAsync("Aluno", "aluno@projetofinal.local", "Aluno@123", "Aluno", tipoAlunoId);

            // =======================
            // Matérias
            // =======================
            if (!await ctx.Materias.AnyAsync())
            {
                ctx.Materias.AddRange(
                    new Materia { Nome = "TI" },
                    new Materia { Nome = "Linguagens" },
                    new Materia { Nome = "Literatura" },
                    new Materia { Nome = "Gastronomia" },
                    new Materia { Nome = "Psicologia" },
                    new Materia { Nome = "Filosofia" },
                    new Materia { Nome = "Ciências Sociais" }
                );
                await ctx.SaveChangesAsync();
            }

            // =======================
            // Tipos de Curso
            // =======================
            if (!await ctx.TiposCurso.AnyAsync())
            {
                ctx.TiposCurso.AddRange(
                    new TipoCurso { Nome = "Técnico" },
                    new TipoCurso { Nome = "Livre" },
                    new TipoCurso { Nome = "Superior" }
                );
                await ctx.SaveChangesAsync();
            }

            // =======================
            // Cursos
            // =======================
            async Task EnsureCursoAsync(
                string nome, string materia, string tipoCurso, int cargaHoraria,
                string? descricao = null, string? imagemUrl = null)
            {
                if (!await ctx.Cursos.AnyAsync(c => c.Nome == nome))
                {
                    ctx.Cursos.Add(new Curso
                    {
                        Nome = nome,
                        IdMateria = await ctx.Materias.Where(m => m.Nome == materia).Select(m => m.Id).FirstAsync(),
                        IdTipoCurso = await ctx.TiposCurso.Where(t => t.Nome == tipoCurso).Select(t => t.Id).FirstAsync(),
                        CargaHoraria = cargaHoraria,
                        Descricao = descricao,
                        ImagemCapaUrl = imagemUrl,
                        DataDeCriacao = DateTime.UtcNow
                    });
                }
            }

            // TI
            await EnsureCursoAsync("Programação em C#", "TI", "Técnico", 40,
                "Curso introdutório de C# focado em lógica e sintaxe básica.", "https://picsum.photos/seed/curso1/400/600");

            await EnsureCursoAsync("Redes de Computadores", "TI", "Livre", 60,
                "Aprenda conceitos de redes, protocolos e infraestrutura.", "https://picsum.photos/seed/curso2/400/600");

            await EnsureCursoAsync("Segurança da Informação", "TI", "Superior", 80,
                "Proteção de dados e práticas de cibersegurança.", "https://picsum.photos/seed/curso3/400/600");

            // Linguagens
            await EnsureCursoAsync("Inglês Básico", "Linguagens", "Livre", 180,
                "Introdução à língua inglesa, vocabulário e gramática.", "https://picsum.photos/seed/curso4/400/600");

            await EnsureCursoAsync("Gramática Avançada", "Linguagens", "Superior", 300,
                "Estudo avançado de gramática e análise sintática.", "https://picsum.photos/seed/curso5/400/600");

            // Filosofia
            await EnsureCursoAsync("Ética e Moral", "Filosofia", "Livre", 90,
                "Reflexões sobre ética, moral e comportamento humano.", "https://picsum.photos/seed/curso6/400/600");

            await EnsureCursoAsync("Filosofia Contemporânea", "Filosofia", "Superior", 100,
                "Estudo das correntes filosóficas do século XX e XXI.", "https://picsum.photos/seed/curso7/400/600");

            // Ciências Sociais
            await EnsureCursoAsync("Sociologia Geral", "Ciências Sociais", "Técnico", 96,
                "Introdução à sociologia: conceitos, teorias e sociedade.", "https://picsum.photos/seed/curso8/400/600");

            await EnsureCursoAsync("Antropologia Cultural", "Ciências Sociais", "Livre", 60,
                "Exploração das culturas humanas e práticas sociais.", "https://picsum.photos/seed/curso9/400/600");

            await EnsureCursoAsync("Política e Sociedade", "Ciências Sociais", "Superior", 120,
                "Análise das estruturas políticas e sociais atuais.", "https://picsum.photos/seed/curso10/400/600");

            if (ctx.ChangeTracker.HasChanges())
                await ctx.SaveChangesAsync();
        }
    }
}
