using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using ProjetoFinal.Infrastructure.Entities;     // ✅ ApplicationUser (entidade Identity)
using ProjetoFinal.Web.Models;                 // ✅ LoginVm (ViewModel de login)

namespace ProjetoFinal.Web.Controllers;

// ✅ Controller fino e moderno: usa primary-constructor para injetar os serviços do Identity.
public class AuthController(
    SignInManager<ApplicationUser> signIn,
    UserManager<ApplicationUser> users) : Controller
{
    // 🔒 Campos readonly que referenciam os serviços injetados
    private readonly SignInManager<ApplicationUser> _signIn = signIn;
    private readonly UserManager<ApplicationUser> _users = users;

    // ============================================================
    // GET /Auth/Login?returnUrl=...
    // ============================================================
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
        => View(new LoginVm { ReturnUrl = returnUrl });

    // ============================================================
    // POST /Auth/Login
    // ============================================================
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm model, CancellationToken ct)
    {
        // Se o modelo não passou nas validações (ex: campos obrigatórios)
        if (!ModelState.IsValid)
            return View(model);

        // Busca o usuário pelo e-mail
        var user = await _users.FindByEmailAsync(model.Email);

        // Checa se o usuário existe e está ativo (caso sua ApplicationUser tenha IsActive)
        if (user is null || (user is { } && (user as dynamic).IsActive == false))
        {
            ModelState.AddModelError("", "Usuário inválido ou inativo.");
            return View(model);
        }

        // Tenta autenticar o usuário (Identity gerencia hashing, lockout, etc.)
        var result = await _signIn.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Credenciais inválidas.");
            return View(model);
        }

        // Se ReturnUrl é local, redireciona para lá
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        // ✅ Caso contrário, redireciona para Home (seção de cursos, por exemplo)
        return RedirectToAction("Index", "Home", new { section = "cursos" });
    }

    // ============================================================
    // POST /Auth/Logout
    // ============================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // ============================================================
    // GET /Auth/Denied
    // ============================================================
    [HttpGet]
    public IActionResult Denied() => Content("Acesso negado.");
}
