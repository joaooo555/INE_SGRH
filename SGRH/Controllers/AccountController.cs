using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGRH.Data;
using SGRH.Helpers;
using SGRH.Models;
using SGRH.Services;
using System.Security.Claims;

namespace SGRH.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly IAuditoriaService _auditoria;

        public AccountController(AppDbContext db, IPasswordHasher hasher, IAuditoriaService auditoria)
        {
            _db = db;
            _hasher = hasher;
            _auditoria = auditoria;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        /// RF01 — Autenticação de Utilizadores: login através de credenciais individuais.
        /// Fluxo da secção 12.1: autenticação → validação das credenciais → identificação
        /// do perfil e permissões → acesso ao painel correspondente ao perfil.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(model);

            var utilizador = await _db.UtilizadoresSistema
                .Include(u => u.PerfilAcesso)
                .Include(u => u.Colaborador)
                .FirstOrDefaultAsync(u =>
                    u.Username.ToLower() == model.Username.ToLower() ||
                    u.Email.ToLower() == model.Username.ToLower());

            // Credenciais inválidas — mensagem genérica, sem revelar qual campo falhou.
            if (utilizador == null || !_hasher.Verificar(model.Password, utilizador.PasswordHash))
            {
                if (utilizador != null)
                {
                    await _auditoria.RegistarAsync(utilizador.IdUtilizador, "utilizador_sistema",
                        "LOGIN_FALHOU", null, new { motivo = "credenciais_invalidas" }, utilizador.IdColaborador);
                }
                ModelState.AddModelError(string.Empty, "Credenciais inválidas.");
                return View(model);
            }

            // Contas desactivadas ou bloqueadas não autenticam (RF02 — estados da conta).
            if (utilizador.Estado == "Bloqueado")
            {
                await _auditoria.RegistarAsync(utilizador.IdUtilizador, "utilizador_sistema",
                    "LOGIN_FALHOU", null, new { motivo = "conta_bloqueada" }, utilizador.IdColaborador);
                ModelState.AddModelError(string.Empty, "A conta está bloqueada. Contacte o Administrador do Sistema.");
                return View(model);
            }

            if (utilizador.Estado == "Inativo")
            {
                await _auditoria.RegistarAsync(utilizador.IdUtilizador, "utilizador_sistema",
                    "LOGIN_FALHOU", null, new { motivo = "conta_inactiva" }, utilizador.IdColaborador);
                ModelState.AddModelError(string.Empty, "A conta está inactiva. Contacte o Administrador do Sistema.");
                return View(model);
            }

            // Migração automática: hashes legados (PBKDF2) são re-hash em BCrypt no primeiro login bem-sucedido.
            if (!PasswordHelper.EhHashBcrypt(utilizador.PasswordHash))
            {
                utilizador.PasswordHash = PasswordHelper.Hash(model.Password);
            }

            // Claims: identidade + perfil — base do controlo de acesso por perfil (RNF-2 / RT05).
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, utilizador.Username),
                new Claim(ClaimTypes.Email, utilizador.Email),
                new Claim("IdUtilizador", utilizador.IdUtilizador.ToString()),
                new Claim("IdPerfil", utilizador.IdPerfil.ToString()),
                new Claim("FullName", utilizador.Colaborador?.NomeCompleto ?? utilizador.Username),
                new Claim("PerfilNome", utilizador.PerfilAcesso?.Nome ?? "Utilizador"),
                new Claim(ClaimTypes.Role, utilizador.PerfilAcesso?.Nome ?? "Utilizador")
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

            utilizador.UltimoAcesso = DateTime.Now;
            await _db.SaveChangesAsync();

            await _auditoria.RegistarAsync(utilizador.IdUtilizador, "utilizador_sistema",
                "LOGIN", null, new { perfil = utilizador.PerfilAcesso?.Nome }, utilizador.IdColaborador);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
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
                    IdPerfil = 5,
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
            var idUtilizador = int.TryParse(User.FindFirstValue("IdUtilizador"), out var id) ? id : (int?)null;

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (idUtilizador.HasValue)
                await _auditoria.RegistarAsync(idUtilizador, "utilizador_sistema", "LOGOUT", null, null);

            return RedirectToAction("Login");
        }
    }
}
