using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGRH.Data;
using SGRH.Helpers;
using SGRH.Models;
using System.Security.Claims;

namespace SGRH.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                var utilizador = await _db.UtilizadoresSistema
                    .Include(u => u.PerfilAcesso)
                    .Include(u => u.Colaborador)
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.Estado == "Ativo");

                if (utilizador != null && PasswordHelper.Verificar(model.Password, utilizador.PasswordHash))
                {
                    if (!PasswordHelper.EhHashBcrypt(utilizador.PasswordHash))
                    {
                        utilizador.PasswordHash = PasswordHelper.Hash(model.Password);
                        await _db.SaveChangesAsync();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Role, utilizador.PerfilAcesso?.Nome ?? "Utilizador"),
                        new Claim("UserId", utilizador.IdUtilizador.ToString()),
                        new Claim("IdPerfil", utilizador.IdPerfil.ToString()),
                        new Claim("PerfilNome", utilizador.PerfilAcesso?.Nome ?? "Utilizador")
                    };

                    if (utilizador.Colaborador != null)
                    {
                        claims.Add(new Claim("ColaboradorNome", utilizador.Colaborador.NomeCompleto));
                    }

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                        });

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Username ou password incorretos.");
            }

            return View(model);
        }

        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                var usernameBase = model.Email.Split('@')[0].Replace(" ", "");
                var username = usernameBase;
                var sufixo = 1;
                while (await _db.UtilizadoresSistema.AnyAsync(u => u.Username == username))
                {
                    username = usernameBase + sufixo;
                    sufixo++;
                }

                var utilizador = new UtilizadorSistema
                {
                    Username = username,
                    Email = model.Email,
                    PasswordHash = PasswordHelper.Hash(model.Password),
                    IdPerfil = 4,
                    Estado = "Ativo",
                    DataCriacao = DateTime.Now
                };

                _db.UtilizadoresSistema.Add(utilizador);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Conta criada com sucesso! Pode fazer login.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
