using ProjetoFinal.Infrastructure;
using ProjetoFinal.Infrastructure.Persistence.Seed;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Force Kestrel to listen on fixed URLs so the Web client can reliably connect
builder.WebHost.UseUrls("http://localhost:5083", "https://localhost:7028");

//1. Configurações de Infraestrutura (DbContext, Identity, Repos/Services)
// Esse método deve configurar o banco de dados usando a ConnectionString
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//2. Configuração do CORS (Ajustado para as portas da sua Web)
// A Web (7000) precisa de permissão para falar com a API (7058)
var allowed = builder.Configuration
 .GetSection("Cors:AllowedOrigins")
 .Get<string[]>()
 ?? new[] { "https://localhost:7000", "http://localhost:5212" };

builder.Services.AddCors(o =>
 o.AddPolicy("Default", p => p.WithOrigins(allowed)
 .AllowAnyHeader()
 .AllowAnyMethod()));

var app = builder.Build();

// --- PIPELINE DE EXECUÇÃO (A ORDEM É VITAL) ---

// CORS deve vir antes de Authentication
app.UseCors("Default");

// Necessário para o sistema de Cadastro e Login
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
 app.UseSwagger();
 app.UseSwaggerUI();
}

// Loga as URLs em que a API está escutando (útil para debugging)
try
{
 var urls = app.Urls;
 if (urls != null && urls.Any())
 {
 Console.WriteLine("[INFO] API will listen on: " + string.Join(", ", urls));
 }
 else
 {
 Console.WriteLine("[INFO] API listening URLs not available in app.Urls. Check environment or launch settings.");
 }
}
catch (Exception ex)
{
 Console.WriteLine("[WARN] Failed to read application URLs: " + ex.Message);
}

// Endpoint de health-check atualizado com a sua porta correta
app.MapGet("/", () => Results.Ok(new
{
 name = "ProjetoFinal.Api",
 status = "ok",
 urls = app.Urls
}));

app.MapControllers();

//3. Aplicação Automática de Banco (Migrate + Seed)
// O try-catch evita que a API feche sozinha se a String de Conexão estiver errada
try
{
 await DatabaseSeeder.EnsureSeededAsync(app.Services);
}
catch (Exception ex)
{
 Console.WriteLine($"[AVISO] Não foi possível rodar o Seed: {ex.Message}");
 }

app.Run();