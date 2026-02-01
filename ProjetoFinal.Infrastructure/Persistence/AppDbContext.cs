using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ProjetoFinal.Domain.Entities;
using ProjetoFinal.Infrastructure.Entities;

namespace ProjetoFinal.Infrastructure.Persistence;

public class AppDbContext :
    IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // ===== ENTIDADES DO DOMÍNIO =====
    public DbSet<Materia> Materias => Set<Materia>();
    public DbSet<TipoCurso> TiposCurso => Set<TipoCurso>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<TipoUsuario> TiposUsuario => Set<TipoUsuario>();


    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // ===== NOMES DAS TABELAS =====
        b.Entity<Materia>().ToTable("Materias");
        b.Entity<TipoCurso>().ToTable("TiposCurso");
        b.Entity<Curso>().ToTable("Cursos");
        b.Entity<TipoUsuario>().ToTable("TiposUsuario");

        // ===== CONFIGURAÇÃO DE CAMPOS =====
        b.Entity<Materia>()
            .Property(p => p.Nome)
            .HasMaxLength(100)
            .IsRequired();

        b.Entity<TipoCurso>()
            .Property(p => p.Nome)
            .HasMaxLength(100)
            .IsRequired();

        b.Entity<TipoCurso>()
            .Property(p => p.Descricao)
            .HasMaxLength(255);

        b.Entity<Curso>()
            .Property(p => p.Nome)
            .HasMaxLength(150)
            .IsRequired();

        b.Entity<Curso>()
            .Property(p => p.Descricao)
            .HasMaxLength(300);

        // ===== RELACIONAMENTOS =====
        // Curso → Matéria (1:N)
        b.Entity<Curso>()
            .HasOne(c => c.Materia)
            .WithMany(m => m.Cursos)
            .HasForeignKey(c => c.IdMateria)
            .OnDelete(DeleteBehavior.Restrict);

        // Curso → TipoCurso (1:N)
        b.Entity<Curso>()
            .HasOne(c => c.TipoCurso)
            .WithMany(t => t.Cursos)
            .HasForeignKey(c => c.IdTipoCurso)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== ÍNDICES =====
        b.Entity<Curso>().HasIndex(c => c.Nome);
        b.Entity<Curso>().HasIndex(c => new { c.IdMateria, c.IdTipoCurso });

        // ===== ApplicationUser (Identity) =====
        b.Entity<ApplicationUser>().ToTable("AspNetUsers");
        b.Entity<ApplicationUser>().HasIndex(u => u.Email).IsUnique();
        b.Entity<ApplicationUser>().HasQueryFilter(u => u.IsActive);
    }
}
