using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGRH.Data;
using SGRH.Models;
using SGRH.Services;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SGRH.Controllers
{
    /// <summary>
    /// Módulo "Administração do Sistema" — área exclusiva do Perfil: Administrador do Sistema.
    /// Espelha o documento SGRH_Requisitos_Atualizado.docx:
    ///   RF02 — Gestão de Utilizadores (criar, editar, activar, desactivar e bloquear contas)
    ///   RF03 — Gestão de Perfis (criar e gerir perfis de utilizadores)
    ///   RF04 — Gestão de Permissões (associar permissões aos perfis e controlar o acesso aos módulos)
    ///   RT08 — Auditoria / Logs (todas as operações relevantes registadas em logs)
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly IAuditoriaService _auditoria;

        // Módulos do sistema conforme a "Matriz geral de acesso aos módulos" do documento
        public static readonly string[] Modulos =
        {
            "Cadastro e Processo Individual",
            "Recrutamento e Selecção",
            "Gestão de Contratos",
            "Administração de Pessoal",
            "Formação e Desenvolvimento",
            "Gestão de Estágios",
            "Clima Organizacional",
            "Assuntos Sociais e Bem-estar",
            "Reporting e Analytics",
            "Administração do Sistema"
        };

        public AdminController(AppDbContext db, IPasswordHasher hasher, IAuditoriaService auditoria)
        {
            _db = db;
            _hasher = hasher;
            _auditoria = auditoria;
        }

        private int? IdUtilizadorActual =>
            int.TryParse(User.FindFirstValue("IdUtilizador"), out var id) ? id : null;

        private string UsernameActual =>
            User.FindFirstValue(ClaimTypes.Name) ?? "sistema";

        // ────────────────────────────────────────────────────────────
        // PÁGINA PRINCIPAL — Painel de Administração do Sistema
        // ────────────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            ViewBag.ActivePage = "AdminSistema";

            var vm = new AdminDashboardViewModel
            {
                TotalUtilizadores = await _db.UtilizadoresSistema.CountAsync(),
                UtilizadoresActivos = await _db.UtilizadoresSistema.CountAsync(u => u.Estado == "Ativo"),
                UtilizadoresBloqueados = await _db.UtilizadoresSistema.CountAsync(u => u.Estado == "Bloqueado"),
                UtilizadoresInactivos = await _db.UtilizadoresSistema.CountAsync(u => u.Estado == "Inativo"),
                TotalPerfis = await _db.PerfisAcesso.CountAsync(),
                AcessosHoje = await _db.LogsAuditoria
                    .Where(l => l.Operacao == "LOGIN" && l.DataHora.Date == DateTime.Today)
                    .CountAsync(),
                OperacoesHoje = await _db.LogsAuditoria
                    .Where(l => l.DataHora.Date == DateTime.Today)
                    .CountAsync(),
                UltimosAcessos = await _db.LogsAuditoria
                    .Include(l => l.UtilizadorSistema)
                    .Where(l => l.Operacao == "LOGIN")
                    .OrderByDescending(l => l.DataHora)
                    .Take(6)
                    .ToListAsync()
            };

            return View(vm);
        }

        // ────────────────────────────────────────────────────────────
        // RF02 — GESTÃO DE UTILIZADORES
        // ────────────────────────────────────────────────────────────
        public async Task<IActionResult> Utilizadores(string? termo, string? estado)
        {
            ViewBag.ActivePage = "AdminSistema";

            var query = _db.UtilizadoresSistema
                .Include(u => u.PerfilAcesso)
                .Include(u => u.Colaborador)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(termo))
                query = query.Where(u =>
                    u.Username.ToLower().Contains(termo.ToLower()) ||
                    u.Email.ToLower().Contains(termo.ToLower()) ||
                    (u.Colaborador != null && u.Colaborador.NomeCompleto.ToLower().Contains(termo.ToLower())));

            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
                query = query.Where(u => u.Estado == estado);

            ViewBag.Termo = termo;
            ViewBag.EstadoFiltro = estado ?? "Todos";
            ViewBag.Perfis = new SelectList(await _db.PerfisAcesso.OrderBy(p => p.Nome).ToListAsync(), "IdPerfil", "Nome");
            ViewBag.Colaboradores = new SelectList(
                await _db.Colaboradores.OrderBy(c => c.NomeCompleto).ToListAsync(),
                "IdColaborador", "NomeCompleto");

            var utilizadores = await query.OrderBy(u => u.Username).ToListAsync();
            return View(utilizadores);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarUtilizador(string username, string email, int idPerfil,
            int? idColaborador, string senha)
        {
            username = username?.Trim() ?? "";
            email = email?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha))
            {
                TempData["Erro"] = "Utilizador, email e senha são obrigatórios.";
                return RedirectToAction(nameof(Utilizadores));
            }

            if (await _db.UtilizadoresSistema.AnyAsync(u => u.Username == username))
            {
                TempData["Erro"] = $"O nome de utilizador «{username}» já existe.";
                return RedirectToAction(nameof(Utilizadores));
            }

            if (await _db.UtilizadoresSistema.AnyAsync(u => u.Email == email))
            {
                TempData["Erro"] = $"O email «{email}» já está em uso.";
                return RedirectToAction(nameof(Utilizadores));
            }

            var utilizador = new UtilizadorSistema
            {
                Username = username,
                Email = email,
                IdPerfil = idPerfil,
                IdColaborador = idColaborador,
                PasswordHash = _hasher.Hash(senha),
                Estado = "Ativo",
                DataCriacao = DateTime.Now
            };

            _db.UtilizadoresSistema.Add(utilizador);
            await _db.SaveChangesAsync();

            await _auditoria.RegistarAsync(IdUtilizadorActual, "utilizador_sistema", "CRIACAO",
                null,
                new { utilizador.IdUtilizador, utilizador.Username, utilizador.Email, utilizador.IdPerfil });

            TempData["Sucesso"] = $"Conta «{username}» criada com sucesso.";
            return RedirectToAction(nameof(Utilizadores));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUtilizador(int id, string email, int idPerfil, int? idColaborador, string? novaSenha)
        {
            var utilizador = await _db.UtilizadoresSistema.FindAsync(id);
            if (utilizador == null)
                return NotFound();

            var antes = new { utilizador.Email, utilizador.IdPerfil, utilizador.IdColaborador };

            utilizador.Email = email?.Trim() ?? utilizador.Email;
            utilizador.IdPerfil = idPerfil;
            utilizador.IdColaborador = idColaborador;

            if (!string.IsNullOrWhiteSpace(novaSenha))
                utilizador.PasswordHash = _hasher.Hash(novaSenha);

            await _db.SaveChangesAsync();

            await _auditoria.RegistarAsync(IdUtilizadorActual, "utilizador_sistema", "ALTERACAO",
                antes,
                new { utilizador.Email, utilizador.IdPerfil, utilizador.IdColaborador, senhaAlterada = !string.IsNullOrWhiteSpace(novaSenha) },
                utilizador.IdUtilizador);

            TempData["Sucesso"] = $"Conta «{utilizador.Username}» actualizada.";
            return RedirectToAction(nameof(Utilizadores));
        }

        /// <summary>RF02 — Activar, desactivar e bloquear utilizadores.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarEstado(int id, string estado)
        {
            var estadosValidos = new[] { "Ativo", "Inativo", "Bloqueado" };
            if (!estadosValidos.Contains(estado))
            {
                TempData["Erro"] = "Estado inválido.";
                return RedirectToAction(nameof(Utilizadores));
            }

            var utilizador = await _db.UtilizadoresSistema.FindAsync(id);
            if (utilizador == null)
                return NotFound();

            if (utilizador.IdUtilizador == IdUtilizadorActual)
            {
                TempData["Erro"] = "Não pode alterar o estado da sua própria conta.";
                return RedirectToAction(nameof(Utilizadores));
            }

            var estadoAnterior = utilizador.Estado;
            utilizador.Estado = estado;
            await _db.SaveChangesAsync();

            await _auditoria.RegistarAsync(IdUtilizadorActual, "utilizador_sistema", "ALTERACAO",
                new { estado = estadoAnterior },
                new { estado = estado },
                utilizador.IdUtilizador);

            TempData["Sucesso"] = $"Conta «{utilizador.Username}» alterada para «{estado}».";
            return RedirectToAction(nameof(Utilizadores));
        }

        /// <summary>Regra de segurança — "garantir que apenas utilizadores autorizados tenham acesso aos dados".</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUtilizador(int id)
        {
            var utilizador = await _db.UtilizadoresSistema
                .Include(u => u.LogsAuditoria)
                .FirstOrDefaultAsync(u => u.IdUtilizador == id);

            if (utilizador == null)
                return NotFound();

            if (utilizador.IdUtilizador == IdUtilizadorActual)
            {
                TempData["Erro"] = "Não pode eliminar a sua própria conta.";
                return RedirectToAction(nameof(Utilizadores));
            }

            if (utilizador.LogsAuditoria.Count > 0)
            {
                // Preserva a rastreabilidade: contas com histórico de auditoria são desactivadas, não eliminadas.
                utilizador.Estado = "Inativo";
                await _db.SaveChangesAsync();
                await _auditoria.RegistarAsync(IdUtilizadorActual, "utilizador_sistema", "ALTERACAO",
                    null, new { accao = "desativacao_em_substituicao_a_eliminacao" },
                    utilizador.IdUtilizador);
                TempData["Sucesso"] = $"O utilizador «{utilizador.Username}» tem histórico de auditoria — foi desactivado em vez de eliminado (rastreabilidade preservada).";
                return RedirectToAction(nameof(Utilizadores));
            }

            var nome = utilizador.Username;
            _db.UtilizadoresSistema.Remove(utilizador);
            await _db.SaveChangesAsync();

            await _auditoria.RegistarAsync(IdUtilizadorActual, "utilizador_sistema", "ELIMINACAO",
                new { username = nome }, null);

            TempData["Sucesso"] = $"Conta «{nome}» eliminada.";
            return RedirectToAction(nameof(Utilizadores));
        }

        // ────────────────────────────────────────────────────────────
        // RF03 — GESTÃO DE PERFIS
        // ────────────────────────────────────────────────────────────
        public async Task<IActionResult> Perfis()
        {
            ViewBag.ActivePage = "AdminSistema";

            var perfis = await _db.PerfisAcesso
                .Include(p => p.Permissoes)
                .Include(p => p.Utilizadores)
                .OrderBy(p => p.IdPerfil)
                .ToListAsync();

            return View(perfis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarPerfil(string nome, string? descricao, int nivelConfidencialidade)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                TempData["Erro"] = "O nome do perfil é obrigatório.";
                return RedirectToAction(nameof(Perfis));
            }

            if (await _db.PerfisAcesso.AnyAsync(p => p.Nome == nome.Trim()))
            {
                TempData["Erro"] = $"Já existe um perfil com o nome «{nome}».";
                return RedirectToAction(nameof(Perfis));
            }

            var perfil = new PerfilAcesso
            {
                Nome = nome.Trim(),
                Descricao = descricao,
                NivelConfidencialidade = nivelConfidencialidade
            };

            _db.PerfisAcesso.Add(perfil);
            await _db.SaveChangesAsync();

            // Por defeito, o novo perfil não tem acesso a nenhum módulo (princípio do menor privilégio).
            await _auditoria.RegistarAsync(IdUtilizadorActual, "perfil_acesso", "CRIACAO",
                null,
                new { perfil.IdPerfil, perfil.Nome, perfil.NivelConfidencialidade });

            TempData["Sucesso"] = $"Perfil «{perfil.Nome}» criado. Associe agora as permissões aos módulos.";
            return RedirectToAction(nameof(Perfis));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(int id, string nome, string? descricao, int nivelConfidencialidade)
        {
            var perfil = await _db.PerfisAcesso.FindAsync(id);
            if (perfil == null)
                return NotFound();

            var antes = new { perfil.Nome, perfil.Descricao, perfil.NivelConfidencialidade };

            perfil.Nome = nome.Trim();
            perfil.Descricao = descricao;
            perfil.NivelConfidencialidade = nivelConfidencialidade;

            await _db.SaveChangesAsync();

            await _auditoria.RegistarAsync(IdUtilizadorActual, "perfil_acesso", "ALTERACAO",
                antes,
                new { perfil.Nome, perfil.Descricao, perfil.NivelConfidencialidade });

            TempData["Sucesso"] = $"Perfil «{perfil.Nome}» actualizado.";
            return RedirectToAction(nameof(Perfis));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPerfil(int id)
        {
            var perfil = await _db.PerfisAcesso
                .Include(p => p.Utilizadores)
                .FirstOrDefaultAsync(p => p.IdPerfil == id);

            if (perfil == null)
                return NotFound();

            if (perfil.Utilizadores.Count > 0)
            {
                TempData["Erro"] = $"O perfil «{perfil.Nome}» tem {perfil.Utilizadores.Count} utilizador(es) associado(s). Reassocie-os antes de eliminar.";
                return RedirectToAction(nameof(Perfis));
            }

            var permissoes = _db.Permissoes.Where(p => p.IdPerfil == id);
            _db.Permissoes.RemoveRange(permissoes);

            _db.PerfisAcesso.Remove(perfil);
            await _db.SaveChangesAsync();

            await _auditoria.RegistarAsync(IdUtilizadorActual, "perfil_acesso", "ELIMINACAO",
                new { perfil.IdPerfil, perfil.Nome }, null);

            TempData["Sucesso"] = $"Perfil «{perfil.Nome}» eliminado.";
            return RedirectToAction(nameof(Perfis));
        }

        // ────────────────────────────────────────────────────────────
        // RF04 — GESTÃO DE PERMISSÕES (perfil × módulo × operação)
        // ────────────────────────────────────────────────────────────
        public async Task<IActionResult> Permissoes(int id)
        {
            ViewBag.ActivePage = "AdminSistema";

            var perfil = await _db.PerfisAcesso
                .Include(p => p.Permissoes)
                .FirstOrDefaultAsync(p => p.IdPerfil == id);

            if (perfil == null)
                return NotFound();

            return View(perfil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarPermissoes(int id, IFormCollection form)
        {
            var perfil = await _db.PerfisAcesso
                .Include(p => p.Permissoes)
                .FirstOrDefaultAsync(p => p.IdPerfil == id);

            if (perfil == null)
                return NotFound();

            var antes = perfil.Permissoes
                .Select(p => new { p.Modulo, p.PodeVisualizar, p.PodeCriar, p.PodeEditar, p.PodeEliminar })
                .ToList();

            _db.Permissoes.RemoveRange(perfil.Permissoes);

            for (var i = 0; i < Modulos.Length; i++)
            {
                var modulo = Modulos[i];
                var key = $"mod_{i}";
                if (!form.ContainsKey(key))
                    continue; // sem acesso ao módulo — princípio do menor privilégio

                var opcoes = form[key].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).ToHashSet();

                _db.Permissoes.Add(new Permissao
                {
                    IdPerfil = id,
                    Modulo = modulo,
                    PodeVisualizar = true, // aceder ao módulo implica visualizar
                    PodeCriar = opcoes.Contains("criar"),
                    PodeEditar = opcoes.Contains("editar"),
                    PodeEliminar = opcoes.Contains("eliminar")
                });
            }

            await _db.SaveChangesAsync();

            var depois = await _db.Permissoes
                .Where(p => p.IdPerfil == id)
                .Select(p => new { p.Modulo, p.PodeVisualizar, p.PodeCriar, p.PodeEditar, p.PodeEliminar })
                .ToListAsync();

            await _auditoria.RegistarAsync(IdUtilizadorActual, "permissao", "ALTERACAO",
                antes, depois);

            TempData["Sucesso"] = $"Permissões do perfil «{perfil.Nome}» actualizadas.";
            return RedirectToAction(nameof(Permissoes), new { id });
        }

        // ────────────────────────────────────────────────────────────
        // RT08 — AUDITORIA: consulta dos logs e registos de auditoria
        // ────────────────────────────────────────────────────────────
        public async Task<IActionResult> Auditoria(DateTime? de, DateTime? ate, int? utilizador, string? operacao)
        {
            ViewBag.ActivePage = "AdminSistema";

            var query = _db.LogsAuditoria
                .Include(l => l.UtilizadorSistema)
                .AsQueryable();

            if (de.HasValue)
                query = query.Where(l => l.DataHora >= de.Value);
            if (ate.HasValue)
                query = query.Where(l => l.DataHora < ate.Value.AddDays(1));
            if (utilizador.HasValue)
                query = query.Where(l => l.IdUtilizador == utilizador.Value);
            if (!string.IsNullOrWhiteSpace(operacao) && operacao != "Todas")
                query = query.Where(l => l.Operacao == operacao);

            ViewBag.FiltroDe = de?.ToString("yyyy-MM-dd");
            ViewBag.FiltroAte = ate?.ToString("yyyy-MM-dd");
            ViewBag.FiltroOperacao = operacao ?? "Todas";
            ViewBag.Utilizadores = new SelectList(
                await _db.UtilizadoresSistema.OrderBy(u => u.Username).ToListAsync(),
                "IdUtilizador", "Username");
            ViewBag.FiltroUtilizador = utilizador;

            var logs = await query
                .OrderByDescending(l => l.DataHora)
                .Take(500)
                .ToListAsync();

            ViewBag.Total = await query.CountAsync();

            return View(logs);
        }

        /// <summary>Exportação dos logs de auditoria (CSV) — "consultar os logs e registos de auditoria".</summary>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ExportarAuditoria(DateTime? de, DateTime? ate)
        {
            var query = _db.LogsAuditoria.Include(l => l.UtilizadorSistema).AsQueryable();

            if (de.HasValue) query = query.Where(l => l.DataHora >= de.Value);
            if (ate.HasValue) query = query.Where(l => l.DataHora < ate.Value.AddDays(1));

            var logs = await query.OrderByDescending(l => l.DataHora).ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("DataHora;Utilizador;Operacao;Tabela;IPAddress;Dados");
            foreach (var l in logs)
                sb.AppendLine($"{l.DataHora:yyyy-MM-dd HH:mm:ss};{l.UtilizadorSistema?.Username ?? "-"};{l.Operacao};{l.TabelaAfetada};{l.IpAddress};\"{(l.DadosAnteriores ?? l.DadosPosteriores ?? "").Replace("\"", "\"\"")}\"");

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();

            await _auditoria.RegistarAsync(IdUtilizadorActual, "log_auditoria", "EXPORTACAO",
                null, new { total = logs.Count });

            return File(bytes, "text/csv", $"auditoria_sgrh_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }
    }

}
