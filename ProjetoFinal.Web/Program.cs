using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Infrastructure.Persistence;      // AppDbContext
using ProjetoFinal.Infrastructure.Entities;       // ApplicationUser
using ProjetoFinal.Web.Areas.Admin.Services;       // DashboardService
using ProjetoFinal.Web.Services;                    // IApiClient / ApiClient

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------
// 1. Serviços MVC + cache
// -----------------------------------------------------------

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

// JSON case-insensitive e ignora comentários
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.PropertyNameCaseInsensitive = true;
    o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
});

// -----------------------------------------------------------
// 2. DbContext (EF Core)
// -----------------------------------------------------------

var conn = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection não configurada.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(conn));

// -----------------------------------------------------------
// 3. Identity
// -----------------------------------------------------------

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// -----------------------------------------------------------
// 4. Políticas de Autorização
// -----------------------------------------------------------

builder.Services.AddAuthorization(options =>
{
    // Apenas Admin pode acessar Dashboard
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Admin ou Docente podem acessar Gestão
    options.AddPolicy("ManagerOrAdmin", policy =>
        policy.RequireRole("Admin", "Docente"));
});

// -----------------------------------------------------------
// 5. Serviços do projeto
// -----------------------------------------------------------

// Dashboard
builder.Services.AddScoped<DashboardService>();

// API Client (para consumir a API local)
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"]
        ?? throw new InvalidOperationException("ApiBaseUrl não configurada.");
    client.BaseAddress = new Uri(baseUrl);
});

// -----------------------------------------------------------
// 6. Construção do app
// -----------------------------------------------------------

var app = builder.Build();

// -----------------------------------------------------------
// 7. Middleware / pipeline HTTP
// -----------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// -----------------------------------------------------------
// 8. Rotas
// -----------------------------------------------------------

// Rotas de áreas (Admin)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


// Rota padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// -----------------------------------------------------------
// 9. Execução
// -----------------------------------------------------------

app.Run();
