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
    /// Gestão de contas do sistema — área exclusiva do Perfil: Administrador do Sistema.
    /// Espelha o documento SGRH_Requisitos_Atualizado.docx:
    ///   RF02 — Gestão de Utilizadores (criar, editar, activar, desactivar e bloquear contas)
    ///   RT08 — Auditoria / Logs (todas as operações relevantes registadas em logs)
    /// </summary>
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly IAuditoriaService _auditoria;
        private readonly AutorizacaoService _autorizacao;

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

        public AdminController(AppDbContext db, IPasswordHasher hasher, IAuditoriaService auditoria,
            AutorizacaoService autorizacao)
        {
            _db = db;
            _hasher = hasher;
            _auditoria = auditoria;
            _autorizacao = autorizacao;
        }

        private int? IdUtilizadorActual =>
            int.TryParse(User.FindFirstValue("IdUtilizador"), out var id) ? id : null;

        private string UsernameActual =>
            User.FindFirstValue(ClaimTypes.Name) ?? "sistema";

        // ────────────────────────────────────────────────────────────
        // RF02 — GESTÃO DE UTILIZADORES
        // ────────────────────────────────────────────────────────────
        public async Task<IActionResult> Utilizadores(string? termo, string? estado, int? idPerfil, int? idUnidade)
        {
            ViewBag.ActivePage = "Utilizadores";

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

            if (idPerfil is > 0)
                query = query.Where(u => u.IdPerfil == idPerfil);

            // Unidade orgânica: só existe através do colaborador associado, por
            // isso utilizadores sem colaborador não são devolvidos por este filtro.
            if (idUnidade is > 0)
                query = query.Where(u =>
                    u.Colaborador != null && u.Colaborador.IdUnidadeOrganica == idUnidade);

            ViewBag.Termo = termo;
            ViewBag.EstadoFiltro = estado ?? "Todos";
            ViewBag.IdPerfilFiltro = idPerfil;
            ViewBag.IdUnidadeFiltro = idUnidade;
            // Listas dos filtros, em tipos concretos (o mesmo padrão de
            // Views/Home/Colaboradores.cshtml). Os SelectList abaixo continuam
            // a existir porque o modal de edição os consome.
            ViewBag.PerfisFiltro = await _db.PerfisAcesso
                .OrderBy(p => p.IdPerfil)
                .ToListAsync();
            ViewBag.UnidadesFiltro = await _db.UnidadesOrganicas
                .OrderBy(x => x.Nome)
                .ToListAsync();
            ViewBag.Perfis = new SelectList(await _db.PerfisAcesso.OrderBy(p => p.Nome).ToListAsync(), "IdPerfil", "Nome");
            ViewBag.Colaboradores = new SelectList(
                await _db.Colaboradores.OrderBy(c => c.NomeCompleto).ToListAsync(),
                "IdColaborador", "NomeCompleto");

            var utilizadores = await query.OrderBy(u => u.Username).ToListAsync();
            return View(utilizadores);
        }

        // ────────────────────────────────────────────────────────────
        // RF02 — NOVO UTILIZADOR (página dedicada de registo)
        // ────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> NovoUtilizador()
        {
            ViewBag.ActivePage = "Utilizadores";
            ViewBag.Perfis = await _db.PerfisAcesso.OrderBy(p => p.Nome).ToListAsync();
            ViewBag.Colaboradores = await _db.Colaboradores
                .Where(c => c.Estado == "Ativo")
                .OrderBy(c => c.NomeCompleto)
                .ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarUtilizador(string username, string email, int idPerfil,
            int? idColaborador, string senha, string? permissoesJson)
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

            await AplicarPermissoesPerfilAsync(idPerfil, permissoesJson);

            await _auditoria.RegistarAsync(IdUtilizadorActual, "utilizador_sistema", "CRIACAO",
                null,
                new { utilizador.IdUtilizador, utilizador.Username, utilizador.Email, utilizador.IdPerfil });

            TempData["Sucesso"] = $"Conta «{username}» criada com sucesso.";
            return RedirectToAction(nameof(Utilizadores));
        }

        /// <summary>Aplica (ou substitui) as permissões configuradas para o perfil na página de registo.</summary>
        private async Task AplicarPermissoesPerfilAsync(int idPerfil, string? permissoesJson)
        {
            if (string.IsNullOrWhiteSpace(permissoesJson))
                return;

            var permissoes = System.Text.Json.JsonSerializer.Deserialize<List<PermissaoRequest>>(
                permissoesJson,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (permissoes == null || permissoes.Count == 0)
                return;

            permissoes = permissoes.Where(p => !string.IsNullOrWhiteSpace(p.Modulo)).ToList();
            if (permissoes.Count == 0)
                return;

            var existentes = await _db.Permissoes.Where(p => p.IdPerfil == idPerfil).ToListAsync();
            if (existentes.Count > 0)
            {
                _db.Permissoes.RemoveRange(existentes);
                await _db.SaveChangesAsync();
            }

            foreach (var perm in permissoes)
            {
                _db.Permissoes.Add(new Permissao
                {
                    IdPerfil = idPerfil,
                    Modulo = perm.Modulo,
                    PodeVisualizar = perm.PodeVisualizar,
                    PodeCriar = perm.PodeCriar,
                    PodeEditar = perm.PodeEditar,
                    PodeEliminar = perm.PodeEliminar
                });
            }

            await _db.SaveChangesAsync();
            _autorizacao.LimparCache(idPerfil);
        }

        [HttpGet]
        public async Task<IActionResult> ObterPermissoes(int idPerfil)
        {
            var permissoes = await _db.Permissoes
                .Where(p => p.IdPerfil == idPerfil)
                .Select(p => new
                {
                    id = p.IdPermissao,
                    modulo = p.Modulo,
                    podeVisualizar = p.PodeVisualizar,
                    podeCriar = p.PodeCriar,
                    podeEditar = p.PodeEditar,
                    podeEliminar = p.PodeEliminar
                })
                .ToListAsync();

            return Json(permissoes);
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
        // RT08 — AUDITORIA: consulta dos logs e registos de auditoria
        // ────────────────────────────────────────────────────────────
        public async Task<IActionResult> Auditoria(DateTime? de, DateTime? ate, int? utilizador, string? operacao)
        {
            ViewBag.ActivePage = "Auditoria";

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

        public class PermissaoRequest
        {
            public string Modulo { get; set; } = "";
            public bool PodeVisualizar { get; set; }
            public bool PodeCriar { get; set; }
            public bool PodeEditar { get; set; }
            public bool PodeEliminar { get; set; }
        }
    }

}
