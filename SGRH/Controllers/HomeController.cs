using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGRH.Authorization;
using System.Security.Claims;
using SGRH.Data;
using SGRH.Helpers;
using SGRH.Models;
using SGRH.Services;

namespace SGRH.Controllers
{

    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly AutorizacaoService _autorizacao;

        public HomeController(AppDbContext db, IWebHostEnvironment env, AutorizacaoService autorizacao)
        {
            _db = db;
            _env = env;
            _autorizacao = autorizacao;
        }

        public int? ObterIdPerfil()
        {
            var claim = User.FindFirst("IdPerfil");
            if (claim != null && int.TryParse(claim.Value, out var id))
                return id;
            return null;
        }

        private async Task CarregarPermissoesViewBag()
        {
            var idPerfil = ObterIdPerfil();
            if (idPerfil.HasValue)
            {
                var permissoes = await _autorizacao.ObterTodasPermissoes(idPerfil.Value);
                ViewBag.Permissoes = permissoes;
            }
            else
            {
                ViewBag.Permissoes = new Dictionary<string, Permissao>();
            }
        }

        // ── DASHBOARD ─────────────────────────────────────────────
        [VerificarPermissao(Modulo = "Dashboard", Operacao = "Visualizar")]
        public async Task<IActionResult> Index()
        {
            ViewBag.ActivePage = "Dashboard";
            await CarregarPermissoesViewBag();
            return View();
        }

        // ── COLABORADORES ─────────────────────────────────────────
        [VerificarPermissao(Modulo = "Colaboradores", Operacao = "Visualizar")]
        public async Task<IActionResult> Colaboradores()
        {
            ViewBag.ActivePage = "Colaboradores";
            await CarregarPermissoesViewBag();

            var colaboradores = await _db.Colaboradores
                .Include(c => c.UnidadeOrganica)
                .Include(c => c.Categoria)
                .Include(c => c.Carreira)
                .Include(c => c.Funcao)
                .Include(c => c.EstadoCivil)
                .Include(c => c.FormaIngresso)
                .OrderBy(c => c.NomeCompleto)
                .ToListAsync();

            ViewBag.UnidadesOrganicas = await _db.UnidadesOrganicas
                .Where(u => u.Estado == "Ativa")
                .OrderBy(u => u.Nome)
                .ToListAsync();

            return View(colaboradores);
        }

        [HttpGet]
        [VerificarPermissao(Modulo = "Colaboradores", Operacao = "Visualizar")]
        public async Task<IActionResult> PesquisarColaboradores(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
            {
                return Json(new object[0]);
            }

            termo = termo.ToLower();

            var colaboradores = await _db.Colaboradores
                .Include(c => c.UnidadeOrganica)
                .Include(c => c.Categoria)
                .Include(c => c.Carreira)
                .Include(c => c.Funcao)
                .Include(c => c.EstadoCivil)
                .Include(c => c.FormaIngresso)
                .Where(c =>
                    c.NomeCompleto.ToLower().Contains(termo) ||
                    (c.Nuit != null && c.Nuit.ToLower().Contains(termo)) ||
                    (c.ContactoEmail != null && c.ContactoEmail.ToLower().Contains(termo)) ||
                    (c.ContactoTelefonico != null && c.ContactoTelefonico.Contains(termo)) ||
                    (c.NumeroIdentificacao != null && c.NumeroIdentificacao.ToLower().Contains(termo)) ||
                    (c.UnidadeOrganica != null && c.UnidadeOrganica.Nome.ToLower().Contains(termo)) ||
                    (c.Categoria != null && c.Categoria.Descricao.ToLower().Contains(termo)) ||
                    (c.Carreira != null && c.Carreira.Nome.ToLower().Contains(termo)) ||
                    (c.Funcao != null && c.Funcao.Nome.ToLower().Contains(termo)) ||
                    (c.FormaIngresso != null && c.FormaIngresso.Nome.ToLower().Contains(termo))
                )
                .OrderBy(c => c.NomeCompleto)
                .Select(c => new
                {
                    c.IdColaborador,
                    c.NomeCompleto,
                    c.Nuit,
                    c.Sexo,
                    c.DataNascimento,
                    c.DataIngresso,
                    c.Estado,
                    c.ContactoTelefonico,
                    c.ContactoEmail,
                    c.EnderecoResidencia,
                    c.Nacionalidade,
                    c.NumeroIdentificacao,
                    c.FotoPath,
                    c.Observacoes,
                    UnidadeOrganica = c.UnidadeOrganica != null ? c.UnidadeOrganica.Nome : null,
                    Categoria = c.Categoria != null ? c.Categoria.Descricao : null,
                    Carreira = c.Carreira != null ? c.Carreira.Nome : null,
                    Funcao = c.Funcao != null ? c.Funcao.Nome : null,
                    EstadoCivil = c.EstadoCivil != null ? c.EstadoCivil.Nome : null,
                    FormaIngresso = c.FormaIngresso != null ? c.FormaIngresso.Nome : null
                })
                .ToListAsync();

            return Json(colaboradores);
        }

        [VerificarPermissao(Modulo = "Colaboradores", Operacao = "Visualizar")]
        public async Task<IActionResult> NovoColaborador()
        {
            ViewBag.ActivePage = "Colaboradores";
            await CarregarPermissoesViewBag();
            await CarregarDadosReferencia();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerificarPermissao(Modulo = "Colaboradores", Operacao = "Criar")]
        public async Task<IActionResult> NovoColaborador(Colaborador colaborador, IFormFile? foto, List<IFormFile>? documentos, string? tipoContrato, decimal? remuneracao, DateOnly? dataInicioContrato, DateOnly? dataFimContrato, string? estadoContrato)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActivePage = "Colaboradores";
                await CarregarPermissoesViewBag();
                await CarregarDadosReferencia();
                return View(colaborador);
            }

            if (foto != null && foto.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "fotos");
                Directory.CreateDirectory(uploadsDir);
                var ext = Path.GetExtension(foto.FileName);
                var fileName = $"foto_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }
                colaborador.FotoPath = $"/uploads/fotos/{fileName}";
            }

            colaborador.DataRegisto = DateTime.Now;
            _db.Colaboradores.Add(colaborador);
            await _db.SaveChangesAsync();

            if (documentos != null && documentos.Count > 0)
            {
                var docsDir = Path.Combine(_env.WebRootPath, "uploads", "documentos");
                Directory.CreateDirectory(docsDir);

                foreach (var doc in documentos)
                {
                    if (doc.Length > 0)
                    {
                        var ext = Path.GetExtension(doc.FileName);
                        var fileName = $"doc_{colaborador.IdColaborador}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}{ext}";
                        var filePath = Path.Combine(docsDir, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await doc.CopyToAsync(stream);
                        }

                        var docEntidade = new Documento
                        {
                            IdColaborador = colaborador.IdColaborador,
                            IdTipoDocumento = 1,
                            Titulo = TituloSeguro(doc.FileName),
                            Ficheiro = await System.IO.File.ReadAllBytesAsync(filePath),
                            Formato = ext,
                            DataUpload = DateTime.Now
                        };
                        _db.Documentos.Add(docEntidade);
                    }
                }
                await _db.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(tipoContrato) && dataInicioContrato.HasValue)
            {
                var tipoContratoObj = await _db.TiposContrato
                    .FirstOrDefaultAsync(t => t.Nome == tipoContrato);

                if (tipoContratoObj != null)
                {
                    var contrato = new Contrato
                    {
                        IdColaborador = colaborador.IdColaborador,
                        IdTipoContrato = tipoContratoObj.IdTipoContrato,
                        NumeroContrato = await GerarNumeroContrato(),
                        DataInicio = dataInicioContrato.Value,
                        DataFim = dataFimContrato,
                        Remuneracao = remuneracao,
                        Estado = estadoContrato ?? "Vigente",
                        DataRegisto = DateTime.Now
                    };
                    _db.Contratos.Add(contrato);
                    await _db.SaveChangesAsync();
                }
            }

            TempData["Toast"] = "Colaborador registado com sucesso!";
            return RedirectToAction(nameof(Colaboradores));
        }

        [VerificarPermissao(Modulo = "Colaboradores", Operacao = "Visualizar")]
        public async Task<IActionResult> EditarColaborador(int? id)
        {
            ViewBag.ActivePage = "Colaboradores";
            await CarregarPermissoesViewBag();

            if (id == null)
                return NotFound();

            var colaborador = await _db.Colaboradores
                .Include(c => c.UnidadeOrganica)
                .Include(c => c.Categoria)
                .Include(c => c.Carreira)
                .Include(c => c.Funcao)
                .Include(c => c.EstadoCivil)
                .Include(c => c.FormaIngresso)
                .FirstOrDefaultAsync(c => c.IdColaborador == id);

            if (colaborador == null)
                return NotFound();

            var contrato = await _db.Contratos
                .Include(c => c.TipoContrato)
                .Where(c => c.IdColaborador == id)
                .OrderByDescending(c => c.DataRegisto)
                .FirstOrDefaultAsync();

            var documentos = await _db.Documentos
                .Where(d => d.IdColaborador == id)
                .OrderByDescending(d => d.DataUpload)
                .ToListAsync();

            ViewBag.Contrato = contrato;
            ViewBag.Documentos = documentos;
            await CarregarDadosReferencia();
            return View(colaborador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerificarPermissao(Modulo = "Colaboradores", Operacao = "Editar")]
        public async Task<IActionResult> EditarColaborador(int id, Colaborador colaborador, IFormFile? foto, List<IFormFile>? documentos, string? tipoContrato, decimal? remuneracao, DateOnly? dataInicioContrato, DateOnly? dataFimContrato, string? estadoContrato)
        {
            if (id != colaborador.IdColaborador)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.ActivePage = "Colaboradores";
                await CarregarPermissoesViewBag();
                ViewBag.Contrato = await _db.Contratos
                    .Include(c => c.TipoContrato)
                    .Where(c => c.IdColaborador == id)
                    .OrderByDescending(c => c.DataRegisto)
                    .FirstOrDefaultAsync();
                ViewBag.Documentos = await _db.Documentos
                    .Where(d => d.IdColaborador == id)
                    .OrderByDescending(d => d.DataUpload)
                    .ToListAsync();
                await CarregarDadosReferencia();
                return View(colaborador);
            }

            var existente = await _db.Colaboradores.FindAsync(id);
            if (existente == null)
                return NotFound();

            if (foto != null && foto.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "fotos");
                Directory.CreateDirectory(uploadsDir);
                var ext = Path.GetExtension(foto.FileName);
                var fileName = $"foto_{id}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }
                existente.FotoPath = $"/uploads/fotos/{fileName}";
            }

            existente.NomeCompleto = colaborador.NomeCompleto;
            existente.Nuit = colaborador.Nuit;
            existente.DataNascimento = colaborador.DataNascimento;
            existente.Sexo = colaborador.Sexo;
            existente.IdEstadoCivil = colaborador.IdEstadoCivil;
            existente.Nacionalidade = colaborador.Nacionalidade;
            existente.NumeroIdentificacao = colaborador.NumeroIdentificacao;
            existente.ContactoTelefonico = colaborador.ContactoTelefonico;
            existente.ContactoEmail = colaborador.ContactoEmail;
            existente.EnderecoResidencia = colaborador.EnderecoResidencia;
            existente.Estado = colaborador.Estado;
            existente.IdUnidadeOrganica = colaborador.IdUnidadeOrganica;
            existente.IdCategoria = colaborador.IdCategoria;
            existente.IdCarreira = colaborador.IdCarreira;
            existente.IdFuncao = colaborador.IdFuncao;
            existente.DataIngresso = colaborador.DataIngresso;
            existente.IdFormaIngresso = colaborador.IdFormaIngresso;
            existente.Observacoes = colaborador.Observacoes;

            await _db.SaveChangesAsync();

            if (documentos != null && documentos.Count > 0)
            {
                var docsDir = Path.Combine(_env.WebRootPath, "uploads", "documentos");
                Directory.CreateDirectory(docsDir);

                foreach (var doc in documentos)
                {
                    if (doc.Length > 0)
                    {
                        var ext = Path.GetExtension(doc.FileName);
                        var fileName = $"doc_{id}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}{ext}";
                        var filePath = Path.Combine(docsDir, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await doc.CopyToAsync(stream);
                        }

                        var docEntidade = new Documento
                        {
                            IdColaborador = id,
                            IdTipoDocumento = 1,
                            Titulo = TituloSeguro(doc.FileName),
                            Ficheiro = await System.IO.File.ReadAllBytesAsync(filePath),
                            Formato = ext,
                            DataUpload = DateTime.Now
                        };
                        _db.Documentos.Add(docEntidade);
                    }
                }
                await _db.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(tipoContrato) && dataInicioContrato.HasValue)
            {
                var tipoContratoObj = await _db.TiposContrato
                    .FirstOrDefaultAsync(t => t.Nome == tipoContrato);

                if (tipoContratoObj != null)
                {
                    var contratoExistente = await _db.Contratos
                        .Where(c => c.IdColaborador == id)
                        .OrderByDescending(c => c.DataRegisto)
                        .FirstOrDefaultAsync();

                    if (contratoExistente != null)
                    {
                        contratoExistente.IdTipoContrato = tipoContratoObj.IdTipoContrato;
                        contratoExistente.DataInicio = dataInicioContrato.Value;
                        contratoExistente.DataFim = dataFimContrato;
                        contratoExistente.Remuneracao = remuneracao;
                        contratoExistente.Estado = estadoContrato ?? "Vigente";
                    }
                    else
                    {
                        var contrato = new Contrato
                        {
                            IdColaborador = id,
                            IdTipoContrato = tipoContratoObj.IdTipoContrato,
                            NumeroContrato = await GerarNumeroContrato(),
                            DataInicio = dataInicioContrato.Value,
                            DataFim = dataFimContrato,
                            Remuneracao = remuneracao,
                            Estado = estadoContrato ?? "Vigente",
                            DataRegisto = DateTime.Now
                        };
                        _db.Contratos.Add(contrato);
                    }
                    await _db.SaveChangesAsync();
                }
            }

            TempData["Toast"] = "Colaborador atualizado com sucesso!";
            return RedirectToAction(nameof(Colaboradores));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerificarPermissao(Modulo = "Colaboradores", Operacao = "Eliminar")]
        public async Task<IActionResult> EliminarColaborador(int id)
        {
            var colaborador = await _db.Colaboradores.FindAsync(id);
            if (colaborador == null)
                return NotFound();

            colaborador.Estado = "Inativo";
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Colaborador desativado com sucesso!";
            return RedirectToAction(nameof(Colaboradores));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistarCarreira(int idColaborador, string tipo, string tipoMovimento, DateOnly data, int? novaCategoria, int? novaFuncao, string? despacho, decimal? novaRemuneracao, string? observacoes)
        {
            var colaborador = await _db.Colaboradores.FindAsync(idColaborador);
            if (colaborador == null) return NotFound();

            var historico = new HistoricoColaborador
            {
                IdColaborador = idColaborador,
                DataEvento = data,
                TipoEvento = tipoMovimento,
                Descricao = (!string.IsNullOrEmpty(despacho) ? despacho + " — " : "") + (observacoes ?? ""),
                Referencia = despacho,
                IdUnidadeOrganica = colaborador.IdUnidadeOrganica,
                IdCategoria = novaCategoria,
                IdCarreira = colaborador.IdCarreira,
                IdFuncao = novaFuncao
            };
            _db.HistoricosColaborador.Add(historico);

            if (novaCategoria.HasValue)
                colaborador.IdCategoria = novaCategoria.Value;
            if (novaFuncao.HasValue)
                colaborador.IdFuncao = novaFuncao.Value;

            await _db.SaveChangesAsync();

            TempData["Toast"] = (tipo == "progressao" ? "Progressão" : "Regressão") + " registada com sucesso!";
            return RedirectToAction(nameof(Colaboradores));
        }

        private async Task CarregarDadosReferencia()
        {
            ViewBag.EstadosCivis = await _db.EstadosCivis.ToListAsync();
            ViewBag.UnidadesOrganicas = await _db.UnidadesOrganicas.Where(u => u.Estado == "Ativa").ToListAsync();
            ViewBag.Categorias = await _db.Categorias.ToListAsync();
            ViewBag.Carreiras = await _db.Carreiras.ToListAsync();
            ViewBag.Funcoes = await _db.Funcoes.ToListAsync();
            ViewBag.FormasIngresso = await _db.FormasIngresso.ToListAsync();
            ViewBag.TiposContrato = await _db.TiposContrato.ToListAsync();
        }

        private async Task<string> GerarNumeroContrato()
        {
            var countAno = await _db.Contratos.CountAsync(c => c.DataRegisto.Year == DateTime.Now.Year);
            return $"CTR-{DateTime.Now.Year}/{(countAno + 1).ToString("D3")}";
        }

        // O título mapeia para nvarchar(150); nomes muito longos rebentariam o SaveChanges
        // com DbUpdateException por truncagem.
        private static string TituloSeguro(string nomeFicheiro)
        {
            var nome = Path.GetFileName(nomeFicheiro);
            return nome.Length <= 150 ? nome : nome[..147] + "...";
        }

        // ── CONTRATOS ─────────────────────────────────────────────
        [VerificarPermissao(Modulo = "Contratos", Operacao = "Visualizar")]
        public async Task<IActionResult> Contratos()
        {
            ViewBag.ActivePage = "Contratos";
            await CarregarPermissoesViewBag();

            var contratos = await _db.Contratos
                .Include(c => c.Colaborador)
                .Include(c => c.TipoContrato)
                .OrderByDescending(c => c.DataRegisto)
                .ToListAsync();

            ViewBag.Vigentes = contratos.Count(c => c.Estado == "Vigente");
            ViewBag.Expirados = contratos.Count(c => c.Estado == "Expirado");

            var hoje = DateOnly.FromDateTime(DateTime.Now);
            var em30Dias = hoje.AddDays(30);
            ViewBag.AExpirar = contratos.Count(c =>
                c.Estado == "Vigente" && c.DataFim.HasValue &&
                c.DataFim.Value >= hoje && c.DataFim.Value <= em30Dias);
            ViewBag.RenovacoesPendentes = ViewBag.AExpirar;
            ViewBag.TiposContrato = await _db.TiposContrato.ToListAsync();

            return View(contratos);
        }

        [HttpGet]
        [VerificarPermissao(Modulo = "Contratos", Operacao = "Visualizar")]
        public async Task<IActionResult> NovoContrato()
        {
            ViewBag.ActivePage = "Contratos";
            await CarregarPermissoesViewBag();
            ViewBag.TiposContrato = await _db.TiposContrato.ToListAsync();
            ViewBag.Colaboradores = await _db.Colaboradores
                .Where(c => c.Estado == "Ativo")
                .OrderBy(c => c.NomeCompleto)
                .ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerificarPermissao(Modulo = "Contratos", Operacao = "Criar")]
        public async Task<IActionResult> NovoContrato(Contrato contrato, string? estadoContrato, List<IFormFile>? documentos)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActivePage = "Contratos";
                await CarregarPermissoesViewBag();
                ViewBag.TiposContrato = await _db.TiposContrato.ToListAsync();
                ViewBag.Colaboradores = await _db.Colaboradores
                    .Where(c => c.Estado == "Ativo")
                    .OrderBy(c => c.NomeCompleto)
                    .ToListAsync();
                return View(contrato);
            }

            contrato.NumeroContrato = await GerarNumeroContrato();
            contrato.Estado = estadoContrato ?? "Vigente";
            contrato.DataRegisto = DateTime.Now;
            _db.Contratos.Add(contrato);
            await _db.SaveChangesAsync();

            if (documentos != null && documentos.Count > 0)
            {
                var docsDir = Path.Combine(_env.WebRootPath, "uploads", "documentos");
                Directory.CreateDirectory(docsDir);

                foreach (var doc in documentos)
                {
                    if (doc.Length > 0)
                    {
                        var ext = Path.GetExtension(doc.FileName);
                        var fileName = $"contrato_{contrato.IdContrato}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}{ext}";
                        var filePath = Path.Combine(docsDir, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await doc.CopyToAsync(stream);
                        }

                        var docEntidade = new Documento
                        {
                            IdColaborador = contrato.IdColaborador,
                            IdTipoDocumento = 1,
                            Titulo = TituloSeguro(doc.FileName),
                            Ficheiro = await System.IO.File.ReadAllBytesAsync(filePath),
                            Formato = ext,
                            DataUpload = DateTime.Now
                        };
                        _db.Documentos.Add(docEntidade);
                    }
                }
                await _db.SaveChangesAsync();
            }

            TempData["Toast"] = "Contrato registado com sucesso!";
            return RedirectToAction(nameof(Contratos));
        }

        [HttpGet]
        [VerificarPermissao(Modulo = "Contratos", Operacao = "Visualizar")]
        public async Task<IActionResult> EditarContrato(int? id)
        {
            if (id == null) return NotFound();

            ViewBag.ActivePage = "Contratos";
            await CarregarPermissoesViewBag();

            var contrato = await _db.Contratos
                .Include(c => c.Colaborador)
                .Include(c => c.TipoContrato)
                .FirstOrDefaultAsync(c => c.IdContrato == id);

            if (contrato == null) return NotFound();

            ViewBag.TiposContrato = await _db.TiposContrato.ToListAsync();
            return View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerificarPermissao(Modulo = "Contratos", Operacao = "Editar")]
        public async Task<IActionResult> EditarContrato(Contrato model, string? estadoContrato, List<IFormFile>? documentos)
        {
            var contrato = await _db.Contratos.FindAsync(model.IdContrato);
            if (contrato == null) return NotFound();

            contrato.IdTipoContrato = model.IdTipoContrato;
            contrato.DataInicio = model.DataInicio;
            contrato.DataFim = model.DataFim;
            contrato.Remuneracao = model.Remuneracao;
            contrato.Objecto = model.Objecto;
            contrato.Estado = estadoContrato ?? contrato.Estado;

            await _db.SaveChangesAsync();

            if (documentos != null && documentos.Count > 0)
            {
                var docsDir = Path.Combine(_env.WebRootPath, "uploads", "documentos");
                Directory.CreateDirectory(docsDir);

                foreach (var doc in documentos)
                {
                    if (doc.Length > 0)
                    {
                        var ext = Path.GetExtension(doc.FileName);
                        var fileName = $"contrato_{contrato.IdContrato}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}{ext}";
                        var filePath = Path.Combine(docsDir, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await doc.CopyToAsync(stream);
                        }

                        var docEntidade = new Documento
                        {
                            IdColaborador = contrato.IdColaborador,
                            IdTipoDocumento = 1,
                            Titulo = TituloSeguro(doc.FileName),
                            Ficheiro = await System.IO.File.ReadAllBytesAsync(filePath),
                            Formato = ext,
                            DataUpload = DateTime.Now
                        };
                        _db.Documentos.Add(docEntidade);
                    }
                }
                await _db.SaveChangesAsync();
            }

            TempData["Toast"] = "Contrato atualizado com sucesso!";
            return RedirectToAction(nameof(Contratos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerificarPermissao(Modulo = "Contratos", Operacao = "Eliminar")]
        public async Task<IActionResult> EliminarContrato(int id)
        {
            var contrato = await _db.Contratos.FindAsync(id);
            if (contrato == null) return NotFound();

            _db.Contratos.Remove(contrato);
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Contrato eliminado com sucesso!";
            return RedirectToAction(nameof(Contratos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerificarPermissao(Modulo = "Contratos", Operacao = "Editar")]
        public async Task<IActionResult> RenovarContrato(int id)
        {
            var contrato = await _db.Contratos.FindAsync(id);
            if (contrato == null) return NotFound();

            contrato.DataFim = (contrato.DataFim ?? DateOnly.FromDateTime(DateTime.Now)).AddYears(1);
            contrato.Estado = "Vigente";
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Contrato renovado com sucesso!";
            return RedirectToAction(nameof(Contratos));
        }

// ── ADMINISTRACAO ─────────────────────────────────────────
        [VerificarPermissao(Modulo = "Administracao", Operacao = "Visualizar")]
        /// <summary>
        /// Módulo 4 — Administração de Pessoal.
        /// Gestão de férias, faltas e licenças (registo, actualização, renovação, cancelamento)
        /// e workflow de parecer e aprovação, conforme o documento de requisitos.
        /// </summary>
        public async Task<IActionResult> Administracao()
        {
            ViewBag.ActivePage = "Administracao";
            await CarregarPermissoesViewBag();

            var vm = new AdministracaoPessoalViewModel
            {
                RegistosAusencia = await _db.RegistosAusencia
                    .Include(r => r.Colaborador)
                    .Include(r => r.TipoAusencia)
                    .OrderByDescending(r => r.DataRegisto)
                    .ToListAsync(),

                PedidosAprovacao = await _db.PedidosAprovacao
                    .Include(p => p.Colaborador)
                    .Include(p => p.TipoPedido)
                    .Include(p => p.Aprovacoes)
                    .OrderByDescending(p => p.DataSubmissao)
                    .ToListAsync(),

                Colaboradores = await _db.Colaboradores
                    .Where(c => c.Estado == "Ativo")
                    .OrderBy(c => c.NomeCompleto)
                    .ToListAsync(),

                TiposAusencia = await _db.TiposAusencia.OrderBy(t => t.Nome).ToListAsync(),
                TiposPedido = await _db.TiposPedido.OrderBy(t => t.Nome).ToListAsync()
            };

            return View(vm);
        }

        // ── CRUD: Registos de Ausência (férias, faltas, licenças) ──

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarAusencia(int idColaborador, int idTipoAusencia, DateOnly dataInicio, DateOnly dataFim, string? motivo, IFormFile? documento)
        {
            if (dataFim < dataInicio)
            {
                TempData["Erro"] = "A data de fim não pode ser anterior à data de início.";
                return RedirectToAction(nameof(Administracao));
            }

            var registo = new RegistoAusencia
            {
                IdColaborador = idColaborador,
                IdTipoAusencia = idTipoAusencia,
                DataInicio = dataInicio,
                DataFim = dataFim,
                Motivo = motivo,
                Estado = "Pendente",
                UtilizadorRegisto = int.TryParse(User.FindFirstValue("IdUtilizador"), out var idU) ? idU : null,
                DataRegisto = DateTime.Now
            };

            if (documento != null && documento.Length > 0)
            {
                using var ms = new MemoryStream();
                await documento.CopyToAsync(ms);
                registo.DocumentoSuporte = ms.ToArray();
            }

            _db.RegistosAusencia.Add(registo);
            await _db.SaveChangesAsync();

            TempData["Sucesso"] = "Registo de ausência criado com sucesso.";
            return RedirectToAction(nameof(Administracao));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAusencia(int id, int idTipoAusencia, DateOnly dataInicio, DateOnly dataFim, string? motivo, string estado)
        {
            var registo = await _db.RegistosAusencia.FindAsync(id);
            if (registo == null)
                return NotFound();

            if (dataFim < dataInicio)
            {
                TempData["Erro"] = "A data de fim não pode ser anterior à data de início.";
                return RedirectToAction(nameof(Administracao));
            }

            registo.IdTipoAusencia = idTipoAusencia;
            registo.DataInicio = dataInicio;
            registo.DataFim = dataFim;
            registo.Motivo = motivo;
            registo.Estado = estado;

            await _db.SaveChangesAsync();

            TempData["Sucesso"] = "Registo de ausência actualizado.";
            return RedirectToAction(nameof(Administracao));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarAusencia(int id)
        {
            var registo = await _db.RegistosAusencia.FindAsync(id);
            if (registo == null)
                return NotFound();

            _db.RegistosAusencia.Remove(registo);
            await _db.SaveChangesAsync();

            TempData["Sucesso"] = "Registo de ausência eliminado.";
            return RedirectToAction(nameof(Administracao));
        }

        // ── Workflow: Pedidos de Aprovação (parecer e aprovação) ──

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarPedido(int idColaborador, int idTipoPedido, string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
            {
                TempData["Erro"] = "A descrição do pedido é obrigatória.";
                return RedirectToAction(nameof(Administracao));
            }

            var pedido = new PedidoAprovacao
            {
                IdColaboradorSolicitante = idColaborador,
                IdTipoPedido = idTipoPedido,
                Descricao = descricao.Trim(),
                Estado = "Pendente",
                DataSubmissao = DateTime.Now
            };

            _db.PedidosAprovacao.Add(pedido);
            await _db.SaveChangesAsync();

            TempData["Sucesso"] = "Pedido submetido ao workflow de aprovação.";
            return RedirectToAction(nameof(Administracao));
        }

        /// <summary>Workflow de parecer e aprovação — decisão sobre um pedido pendente.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DecidirPedido(int id, string decisao, string? parecer)
        {
            var pedido = await _db.PedidosAprovacao
                .Include(p => p.Aprovacoes)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
                return NotFound();

            if (pedido.Estado != "Pendente")
            {
                TempData["Erro"] = "Este pedido já foi decidido.";
                return RedirectToAction(nameof(Administracao));
            }

            decisao = decisao == "Aprovado" ? "Aprovado" : "Rejeitado";

            var idAprovador = int.TryParse(User.FindFirstValue("IdColaborador"), out var idC) ? idC : 0;
            var nivel = pedido.Aprovacoes.Count + 1;

            _db.AprovacoesPedido.Add(new AprovacaoPedido
            {
                IdPedido = pedido.IdPedido,
                IdAprovador = idAprovador > 0 ? idAprovador : pedido.IdColaboradorSolicitante,
                NivelAprovacao = nivel,
                Decisao = decisao,
                Parecer = parecer,
                DataDecisao = DateTime.Now
            });

            pedido.Estado = decisao;
            await _db.SaveChangesAsync();

            TempData["Sucesso"] = $"Pedido {decisao.ToLower()} com sucesso.";
            return RedirectToAction(nameof(Administracao));
        }

        /// <summary>
        /// Botões directos da tabela: Aprovar / Rejeitar / (reabrir) Pendente.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MudarEstadoPedido(int id, string estado)
        {
            var pedido = await _db.PedidosAprovacao
                .Include(p => p.Aprovacoes)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
                return NotFound();

            estado = estado switch
            {
                "Aprovado" => "Aprovado",
                "Rejeitado" => "Rejeitado",
                _ => "Pendente"
            };

            if (pedido.Estado == estado)
                return RedirectToAction(nameof(Administracao));

            if (estado == "Pendente")
            {
                // Reabrir o pedido: remove pareceres anteriores e volta a Pendente
                _db.AprovacoesPedido.RemoveRange(pedido.Aprovacoes);
                pedido.Estado = "Pendente";
                TempData["Sucesso"] = "Pedido reaberto (Pendente).";
            }
            else
            {
                if (pedido.Estado != "Pendente")
                    _db.AprovacoesPedido.RemoveRange(pedido.Aprovacoes); // substitui a decisão anterior

                var idAprovador = int.TryParse(User.FindFirstValue("IdColaborador"), out var idC) ? idC : 0;

                _db.AprovacoesPedido.Add(new AprovacaoPedido
                {
                    IdPedido = pedido.IdPedido,
                    IdAprovador = idAprovador > 0 ? idAprovador : pedido.IdColaboradorSolicitante,
                    NivelAprovacao = 1,
                    Decisao = estado,
                    Parecer = null,
                    DataDecisao = DateTime.Now
                });

                pedido.Estado = estado;
                TempData["Sucesso"] = $"Pedido marcado como {estado.ToLower()}.";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Administracao));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPedido(int id)
        {
            var pedido = await _db.PedidosAprovacao.FindAsync(id);
            if (pedido == null)
                return NotFound();

            if (pedido.Estado == "Aprovado")
            {
                TempData["Erro"] = "Não é possível eliminar um pedido já aprovado.";
                return RedirectToAction(nameof(Administracao));
            }

            var aprovacoes = _db.AprovacoesPedido.Where(a => a.IdPedido == id);
            _db.AprovacoesPedido.RemoveRange(aprovacoes);
            _db.PedidosAprovacao.Remove(pedido);
            await _db.SaveChangesAsync();

            TempData["Sucesso"] = "Pedido eliminado.";
            return RedirectToAction(nameof(Administracao));
        }

        // ── CRUD: Tipos (ausência e pedido) — configuração essencial ──

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarTipoAusencia(string nome, string? descricao, int? diasMaximos)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                TempData["Erro"] = "O nome do tipo de ausência é obrigatório.";
                return RedirectToAction(nameof(Administracao));
            }

            _db.TiposAusencia.Add(new TipoAusencia { Nome = nome.Trim(), Descricao = descricao, DiasMaximos = diasMaximos });
            await _db.SaveChangesAsync();

            TempData["Sucesso"] = "Tipo de ausência criado.";
            return RedirectToAction(nameof(Administracao));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarTipoAusencia(int id)
        {
            var tipo = await _db.TiposAusencia.FindAsync(id);
            if (tipo == null)
                return NotFound();

            if (await _db.RegistosAusencia.AnyAsync(r => r.IdTipoAusencia == id))
            {
                TempData["Erro"] = "Este tipo tem registos associados e não pode ser eliminado.";
                return RedirectToAction(nameof(Administracao));
            }

            _db.TiposAusencia.Remove(tipo);
            await _db.SaveChangesAsync();

            TempData["Sucesso"] = "Tipo de ausência eliminado.";
            return RedirectToAction(nameof(Administracao));
        }

        // ── FORMACAO ──────────────────────────────────────────────
        [VerificarPermissao(Modulo = "Formacao", Operacao = "Visualizar")]
        public async Task<IActionResult> Formacao()
        {
            ViewBag.ActivePage = "Formacao";
            await CarregarPermissoesViewBag();
            return View();
        }

        // ── ESTAGIOS ──────────────────────────────────────────────
        [VerificarPermissao(Modulo = "Estagios", Operacao = "Visualizar")]
        public async Task<IActionResult> Estagios()
        {
            ViewBag.ActivePage = "Estagios";
            await CarregarPermissoesViewBag();
            var estagios = await _db.Estagios
                .Include(s => s.UnidadeOrganica)
                .Include(s => s.Supervisor)
                .OrderByDescending(s => s.DataRegisto)
                .ToListAsync();
            ViewBag.Estagios = estagios;
            ViewBag.EstagiosJson = JsonSerializer.Serialize(estagios.Select(s => new
            {
                nome = s.NomeEstagiario,
                initials = Iniciais(s.NomeEstagiario),
                tipo = ChaveTipo(s.TipoEstagio),
                tipoLabel = s.TipoEstagio,
                area = s.UnidadeOrganica?.Nome ?? "—",
                instituicao = s.InstituicaoEnsino ?? "—",
                orientador = s.Supervisor?.NomeCompleto ?? "—",
                inicio = s.DataInicio.ToString("yyyy-MM-dd"),
                fim = s.DataFim?.ToString("yyyy-MM-dd") ?? "",
                estado = ChaveEstado(s.Estado),
                estadoLabel = s.Estado,
                badgeClass = s.Estado switch
                {
                    "Concluido" => "badge-info",
                    "Em Curso" => "badge-success",
                    "Pendente" => "badge-warning",
                    _ => "badge-secondary"
                },
                progresso = s.DataFim.HasValue && s.DataInicio < DateOnly.FromDateTime(DateTime.Today)
                    ? (int)Math.Min(100, Math.Round((DateTime.Today - s.DataInicio.ToDateTime(TimeOnly.MinValue)).TotalDays /
                        (s.DataFim.Value.ToDateTime(TimeOnly.MinValue) - s.DataInicio.ToDateTime(TimeOnly.MinValue)).TotalDays * 100))
                    : 0,
                avaliacao = s.Avaliacoes != null && s.Avaliacoes.Any(),
                avaliacaoTipo = s.Avaliacoes?.FirstOrDefault()?.Nota.ToString() ?? ""
            }));
            return View();
        }

        private static string Iniciais(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return "??";
            var partes = nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return partes.Length >= 2
                ? partes[0][..1] + partes[^1][..1]
                : partes[0][..1];
        }

        private static string ChaveTipo(string? tipo)
        {
            return tipo?.ToLowerInvariant().Trim().Replace(" ", "-")
                .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                .Replace("ã", "a").Replace("õ", "o").Replace("â", "a").Replace("ê", "e").Replace("ô", "o")
                ?? "outro";
        }

        private static string ChaveEstado(string? estado)
        {
            var chave = estado?.ToLowerInvariant().Trim().Replace(" ", "-")
                .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                .Replace("ã", "a").Replace("õ", "o").Replace("â", "a").Replace("ê", "e").Replace("ô", "o") ?? "";
            return chave switch
            {
                "em-curso" => "em-curso",
                "concluido" or "concluída" or "concluido" => "concluido",
                "pendente" or "solicitado" => "pendente",
                _ => string.IsNullOrEmpty(chave) ? "pendente" : chave
            };
        }

        [VerificarPermissao(Modulo = "Estagios", Operacao = "Visualizar")]
        public async Task<IActionResult> NovoEstagio()
        {
            ViewBag.ActivePage = "Estagios";
            await CarregarPermissoesViewBag();
            ViewBag.Unidades = await _db.UnidadesOrganicas
                .Where(u => u.Estado == "Ativa")
                .OrderBy(u => u.Nome)
                .ToListAsync();
            ViewBag.Supervisores = await _db.Colaboradores
                .Where(c => c.Estado == "Ativo")
                .OrderBy(c => c.NomeCompleto)
                .ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerificarPermissao(Modulo = "Estagios", Operacao = "Criar")]
        public async Task<IActionResult> NovoEstagio(string? nomeEstagiario, string? email, string? telefone, string? documentoIdentificacao, string? instituicaoEnsino, string? tipoEstagio, int? idUnidadeOrganica, int? idSupervisor, DateTime? dataInicio, DateTime? dataFim, string? planoEstagio)
        {
            if (string.IsNullOrWhiteSpace(nomeEstagiario) || string.IsNullOrWhiteSpace(tipoEstagio) || !idUnidadeOrganica.HasValue || !idSupervisor.HasValue || !dataInicio.HasValue)
            {
                TempData["Erro"] = "Preencha todos os campos obrigatorios.";
                return RedirectToAction(nameof(NovoEstagio));
            }

            var estagio = new Estagio
            {
                NomeEstagiario = nomeEstagiario.Trim(),
                Email = email?.Trim(),
                Telefone = telefone?.Trim(),
                DocumentoIdentificacao = documentoIdentificacao?.Trim(),
                InstituicaoEnsino = instituicaoEnsino?.Trim(),
                TipoEstagio = tipoEstagio,
                IdUnidadeOrganica = idUnidadeOrganica.Value,
                IdSupervisor = idSupervisor.Value,
                DataInicio = DateOnly.FromDateTime(dataInicio.Value),
                DataFim = dataFim.HasValue ? DateOnly.FromDateTime(dataFim.Value) : null,
                PlanoEstagio = planoEstagio?.Trim(),
                Estado = "Solicitado",
                DeclaracaoEmitida = false,
                DataRegisto = DateTime.Now,
                UtilizadorRegisto = HttpContext.Session.GetInt32("IdUtilizador") ?? 0
            };

            _db.Estagios.Add(estagio);
            await _db.SaveChangesAsync();

            TempData["Toast"] = "Estagio cadastrado com sucesso!";
            return RedirectToAction(nameof(Estagios));
        }

        // ── GUIAS DE MARCHA ───────────────────────────────────────
        [VerificarPermissao(Modulo = "GuiasMarcha", Operacao = "Visualizar")]
        public async Task<IActionResult> GuiasMarcha()
        {
            ViewBag.ActivePage = "GuiasMarcha";
            await CarregarPermissoesViewBag();
            return View();
        }

        // ── RECRUTAMENTO ──────────────────────────────────────────
        [VerificarPermissao(Modulo = "Recrutamento", Operacao = "Visualizar")]
        public async Task<IActionResult> Recrutamento()
        {
            ViewBag.ActivePage = "Recrutamento";
            await CarregarPermissoesViewBag();
            return View();
        }

        // ── CLIMA ─────────────────────────────────────────────────
        [VerificarPermissao(Modulo = "Clima", Operacao = "Visualizar")]
        public async Task<IActionResult> Clima()
        {
            ViewBag.ActivePage = "Clima";
            await CarregarPermissoesViewBag();
            return View();
        }

        // ── ASSUNTOS SOCIAIS ──────────────────────────────────────
        [VerificarPermissao(Modulo = "AssuntosSociais", Operacao = "Visualizar")]
        public async Task<IActionResult> AssuntosSociais()
        {
            ViewBag.ActivePage = "AssuntosSociais";
            await CarregarPermissoesViewBag();
            return View();
        }

        // ── REPORTING ─────────────────────────────────────────────
        [VerificarPermissao(Modulo = "Reporting", Operacao = "Visualizar")]
        public async Task<IActionResult> Reporting()
        {
            ViewBag.ActivePage = "Reporting";
            await CarregarPermissoesViewBag();
            return View();
        }

        // ── UTILIZADORES ──────────────────────────────────────────
        [VerificarPermissao(Modulo = "Utilizadores", Operacao = "Visualizar")]
        public async Task<IActionResult> Utilizadores()
        {
            ViewBag.ActivePage = "Utilizadores";
            await CarregarPermissoesViewBag();
            ViewBag.Perfis = await _db.PerfisAcesso.ToListAsync();
            ViewBag.Colaboradores = await _db.Colaboradores
                .Where(c => c.Estado == "Ativo")
                .OrderBy(c => c.NomeCompleto)
                .Select(c => new { c.IdColaborador, c.NomeCompleto })
                .ToListAsync();
            return View();
        }

        [VerificarPermissao(Modulo = "Utilizadores", Operacao = "Criar")]
        public async Task<IActionResult> NovoUtilizador()
        {
            ViewBag.ActivePage = "Utilizadores";
            await CarregarPermissoesViewBag();
            ViewBag.Perfis = await _db.PerfisAcesso.ToListAsync();
            ViewBag.Colaboradores = await _db.Colaboradores
                .Where(c => c.Estado == "Ativo")
                .OrderBy(c => c.NomeCompleto)
                .ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [VerificarPermissao(Modulo = "Utilizadores", Operacao = "Criar")]
        public async Task<IActionResult> NovoUtilizador(string username, string email, string password, int idPerfil, int? idColaborador, string? permissoesJson)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Erro"] = "Preencha todos os campos obrigatorios.";
                return RedirectToAction(nameof(NovoUtilizador));
            }

            if (await _db.UtilizadoresSistema.AnyAsync(u => u.Username == username))
            {
                TempData["Erro"] = "Username ja existe.";
                return RedirectToAction(nameof(NovoUtilizador));
            }

            var utilizador = new UtilizadorSistema
            {
                Username = username,
                Email = email,
                PasswordHash = PasswordHelper.Hash(password),
                IdPerfil = idPerfil,
                IdColaborador = idColaborador,
                Estado = "Ativo",
                DataCriacao = DateTime.Now
            };

            _db.UtilizadoresSistema.Add(utilizador);
            await _db.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(permissoesJson))
            {
                var permissoes = System.Text.Json.JsonSerializer.Deserialize<List<PermissaoRequest>>(
                    permissoesJson,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (permissoes != null && permissoes.Count > 0)
                {
                    permissoes = permissoes.Where(p => !string.IsNullOrWhiteSpace(p.Modulo)).ToList();
                    if (permissoes.Count > 0)
                    {
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
                }
            }

            TempData["Sucesso"] = "Utilizador criado com sucesso!";
            return RedirectToAction(nameof(Utilizadores));
        }

        [HttpGet]
        [VerificarPermissao(Modulo = "Utilizadores", Operacao = "Visualizar")]
        public async Task<IActionResult> ObterUtilizadores()
        {
            var utilizadores = await _db.UtilizadoresSistema
                .Include(u => u.PerfilAcesso)
                .Include(u => u.Colaborador)
                .OrderBy(u => u.Username)
                .Select(u => new
                {
                    id = u.IdUtilizador,
                    username = u.Username,
                    email = u.Email,
                    perfil = u.PerfilAcesso.Nome,
                    idPerfil = u.IdPerfil,
                    colaborador = u.Colaborador != null ? u.Colaborador.NomeCompleto : "",
                    idColaborador = u.IdColaborador,
                    estado = u.Estado,
                    ultimoAcesso = u.UltimoAcesso,
                    dataCriacao = u.DataCriacao
                })
                .ToListAsync();

            return Json(utilizadores);
        }

        [HttpGet]
        [VerificarPermissao(Modulo = "Utilizadores", Operacao = "Visualizar")]
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
        [VerificarPermissao(Modulo = "Utilizadores", Operacao = "Criar")]
        public async Task<IActionResult> CriarUtilizador([FromBody] CriarUtilizadorRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return Json(new { sucesso = false, mensagem = "Dados incompletos." });

            if (await _db.UtilizadoresSistema.AnyAsync(u => u.Username == model.Username))
                return Json(new { sucesso = false, mensagem = "Username ja existe." });

            var utilizador = new UtilizadorSistema
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = PasswordHelper.Hash(model.Password),
                IdPerfil = model.IdPerfil,
                IdColaborador = model.IdColaborador,
                Estado = "Ativo",
                DataCriacao = DateTime.Now
            };

            _db.UtilizadoresSistema.Add(utilizador);
            await _db.SaveChangesAsync();

            return Json(new { sucesso = true, mensagem = "Utilizador criado com sucesso!" });
        }

        [HttpPost]
        [VerificarPermissao(Modulo = "Utilizadores", Operacao = "Editar")]
        public async Task<IActionResult> AtualizarUtilizador([FromBody] AtualizarUtilizadorRequest model)
        {
            if (model == null) return Json(new { sucesso = false, mensagem = "Dados invalidos." });

            var utilizador = await _db.UtilizadoresSistema.FindAsync(model.Id);
            if (utilizador == null) return Json(new { sucesso = false, mensagem = "Utilizador nao encontrado." });

            utilizador.Email = model.Email;
            utilizador.IdPerfil = model.IdPerfil;
            utilizador.Estado = model.Estado;
            if (!string.IsNullOrWhiteSpace(model.Password))
                utilizador.PasswordHash = PasswordHelper.Hash(model.Password);

            await _db.SaveChangesAsync();
            _autorizacao.LimparCache(model.IdPerfil);

            return Json(new { sucesso = true, mensagem = "Utilizador actualizado com sucesso!" });
        }

        [HttpPost]
        [VerificarPermissao(Modulo = "Utilizadores", Operacao = "Eliminar")]
        public async Task<IActionResult> EliminarUtilizador(int id)
        {
            var utilizador = await _db.UtilizadoresSistema.FindAsync(id);
            if (utilizador == null) return Json(new { sucesso = false, mensagem = "Utilizador nao encontrado." });

            _db.UtilizadoresSistema.Remove(utilizador);
            await _db.SaveChangesAsync();

            return Json(new { sucesso = true, mensagem = "Utilizador eliminado com sucesso!" });
        }

        [HttpPost]
        [VerificarPermissao(Modulo = "Utilizadores", Operacao = "Editar")]
        public async Task<IActionResult> GuardarPermissoes([FromBody] GuardarPermissoesRequest model)
        {
            if (model == null) return Json(new { sucesso = false, mensagem = "Dados invalidos." });

            var permissoesExistentes = await _db.Permissoes
                .Where(p => p.IdPerfil == model.IdPerfil)
                .ToListAsync();

            if (permissoesExistentes.Count > 0)
            {
                _db.Permissoes.RemoveRange(permissoesExistentes);
                await _db.SaveChangesAsync();
            }

            foreach (var perm in model.Permissoes)
            {
                _db.Permissoes.Add(new Permissao
                {
                    IdPerfil = model.IdPerfil,
                    Modulo = perm.Modulo,
                    PodeVisualizar = perm.PodeVisualizar,
                    PodeCriar = perm.PodeCriar,
                    PodeEditar = perm.PodeEditar,
                    PodeEliminar = perm.PodeEliminar
                });
            }

            await _db.SaveChangesAsync();
            _autorizacao.LimparCache(model.IdPerfil);

            return Json(new { sucesso = true, mensagem = "Permissoes actualizadas com sucesso!" });
        }

        public class CriarUtilizadorRequest
        {
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
            public int IdPerfil { get; set; }
            public int? IdColaborador { get; set; }
        }

        public class AtualizarUtilizadorRequest
        {
            public int Id { get; set; }
            public string Email { get; set; } = "";
            public int IdPerfil { get; set; }
            public string Estado { get; set; } = "Ativo";
            public string? Password { get; set; }
        }

        public class GuardarPermissoesRequest
        {
            public int IdPerfil { get; set; }
            public List<PermissaoRequest> Permissoes { get; set; } = [];
        }

        public class PermissaoRequest
        {
            public string Modulo { get; set; } = "";
            public bool PodeVisualizar { get; set; }
            public bool PodeCriar { get; set; }
            public bool PodeEditar { get; set; }
            public bool PodeEliminar { get; set; }
        }

        // ── AUDITORIA ─────────────────────────────────────────────
        [VerificarPermissao(Modulo = "Auditoria", Operacao = "Visualizar")]
        public async Task<IActionResult> Auditoria()
        {
            ViewBag.ActivePage = "Auditoria";
            await CarregarPermissoesViewBag();
            return View();
        }

        // ── CONFIGURACOES ─────────────────────────────────────────
        [VerificarPermissao(Modulo = "Configuracoes", Operacao = "Visualizar")]
        public async Task<IActionResult> Configuracoes()
        {
            ViewBag.ActivePage = "Configuracoes";
            await CarregarPermissoesViewBag();
            return View();
        }
    }
}
