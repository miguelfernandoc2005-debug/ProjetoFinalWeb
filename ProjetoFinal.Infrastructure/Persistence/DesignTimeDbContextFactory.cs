using System;
using System.IO;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProjetoFinal.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
 public AppDbContext CreateDbContext(string[] args)
 {
 // Tenta carregar a connection string a partir de appsettings.json
 // Observação: ao executar 'dotnet ef' a pasta corrente pode ser diferente,
 // então tentamos alguns caminhos comuns (projeto atual e projeto web).
 string? conn = null;

 var basePath = Directory.GetCurrentDirectory();

 conn = GetConnectionStringFromPaths(basePath, new[] { "appsettings.json", Path.Combine("..", "ProjetoFinal.Web", "appsettings.json") });

 if (string.IsNullOrEmpty(conn))
 throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada. Certifique-se de ter 'DefaultConnection' em appsettings.json do projeto Web ou de informar --startup-project ao usar dotnet ef.");

 var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
 optionsBuilder.UseSqlServer(conn);

 return new AppDbContext(optionsBuilder.Options);
 }

 private string? GetConnectionStringFromPaths(string basePath, string[] relativeFiles)
 {
 foreach (var rel in relativeFiles)
 {
 try
 {
 var path = Path.GetFullPath(Path.Combine(basePath, rel));
 if (!File.Exists(path))
 continue;

 var json = File.ReadAllText(path);
 using var doc = JsonDocument.Parse(json);
 if (doc.RootElement.TryGetProperty("ConnectionStrings", out var csElem) &&
 csElem.ValueKind == JsonValueKind.Object &&
 csElem.TryGetProperty("DefaultConnection", out var defaultConn) &&
 defaultConn.ValueKind == JsonValueKind.String)
 {
 var conn = defaultConn.GetString();
 if (!string.IsNullOrEmpty(conn))
 return conn;
 }
 }
 catch
 {
 // ignora e continua
 }
 }

 return null;
 }
}
