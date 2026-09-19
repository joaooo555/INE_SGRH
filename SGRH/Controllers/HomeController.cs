using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGRH.Data;
using SGRH.Helpers;
using SGRH.Models;

namespace SGRH.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public HomeController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            ViewBag.ActivePage = "Dashboard";
            return View();
        }

        public async Task<IActionResult> Colaboradores()
        {
            ViewBag.ActivePage = "Colaboradores";

            var colaboradores = await _db.Colaboradores
                .Include(c => c.UnidadeOrganica)
                .Include(c => c.Categoria)
                .Include(c => c.Carreira)
                .Include(c => c.Funcao)
                .Include(c => c.EstadoCivil)
                .Include(c => c.FormaIngresso)
                .OrderBy(c => c.NomeCompleto)
                .ToListAsync();

            return View(colaboradores);
        }

        [HttpGet]
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

        public async Task<IActionResult> NovoColaborador()
        {
            ViewBag.ActivePage = "Colaboradores";
            await CarregarDadosReferencia();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NovoColaborador(Colaborador colaborador, IFormFile? foto, List<IFormFile>? documentos, string? tipoContrato, decimal? remuneracao, DateOnly? dataInicioContrato, DateOnly? dataFimContrato, string? estadoContrato)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActivePage = "Colaboradores";
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
                            Titulo = doc.FileName,
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

        public async Task<IActionResult> EditarColaborador(int? id)
        {
            ViewBag.ActivePage = "Colaboradores";

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
        public async Task<IActionResult> EditarColaborador(int id, Colaborador colaborador, IFormFile? foto, List<IFormFile>? documentos, string? tipoContrato, decimal? remuneracao, DateOnly? dataInicioContrato, DateOnly? dataFimContrato, string? estadoContrato)
        {
            if (id != colaborador.IdColaborador)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.ActivePage = "Colaboradores";
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
                            Titulo = doc.FileName,
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

        public async Task<IActionResult> Contratos()
        {
            ViewBag.ActivePage = "Contratos";

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
        public async Task<IActionResult> NovoContrato()
        {
            ViewBag.ActivePage = "Contratos";
            ViewBag.TiposContrato = await _db.TiposContrato.ToListAsync();
            ViewBag.Colaboradores = await _db.Colaboradores
                .Where(c => c.Estado == "Ativo")
                .OrderBy(c => c.NomeCompleto)
                .ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NovoContrato(Contrato contrato, string? estadoContrato, List<IFormFile>? documentos)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActivePage = "Contratos";
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
                            Titulo = doc.FileName,
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
        public async Task<IActionResult> EditarContrato(int? id)
        {
            if (id == null) return NotFound();

            ViewBag.ActivePage = "Contratos";

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
                            Titulo = doc.FileName,
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

        public IActionResult Administracao()
        {
            ViewBag.ActivePage = "Administracao";
            return View();
        }

        public IActionResult Formacao()
        {
            ViewBag.ActivePage = "Formacao";
            return View();
        }

        public IActionResult Estagios()
        {
            ViewBag.ActivePage = "Estagios";
            return View();
        }

        public IActionResult GuiasMarcha()
        {
            ViewBag.ActivePage = "GuiasMarcha";
            return View();
        }

        public IActionResult Recrutamento()
        {
            ViewBag.ActivePage = "Recrutamento";
            return View();
        }

        public IActionResult Clima()
        {
            ViewBag.ActivePage = "Clima";
            return View();
        }

        public IActionResult AssuntosSociais()
        {
            ViewBag.ActivePage = "AssuntosSociais";
            return View();
        }

        public IActionResult Reporting()
        {
            ViewBag.ActivePage = "Reporting";
            return View();
        }

        public async Task<IActionResult> Utilizadores()
        {
            ViewBag.ActivePage = "Utilizadores";
            ViewBag.Perfis = await _db.PerfisAcesso.ToListAsync();
            ViewBag.Colaboradores = await _db.Colaboradores
                .Where(c => c.Estado == "Ativo")
                .OrderBy(c => c.NomeCompleto)
                .Select(c => new { c.IdColaborador, c.NomeCompleto })
                .ToListAsync();
            return View();
        }

        public async Task<IActionResult> NovoUtilizador()
        {
            ViewBag.ActivePage = "Utilizadores";
            ViewBag.Perfis = await _db.PerfisAcesso.ToListAsync();
            ViewBag.Colaboradores = await _db.Colaboradores
                .Where(c => c.Estado == "Ativo")
                .OrderBy(c => c.NomeCompleto)
                .ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    }
                }
            }

            TempData["Sucesso"] = "Utilizador criado com sucesso!";
            return RedirectToAction(nameof(Utilizadores));
        }

        [HttpGet]
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
        public async Task<IActionResult> CriarUtilizador([FromBody] CriarUtilizadorRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return Json(new { sucesso = false, mensagem = "Dados incompletos." });

            if (await _db.UtilizadoresSistema.AnyAsync(u => u.Username == model.Username))
                return Json(new { sucesso = false, mensagem = "Username já existe." });

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
        public async Task<IActionResult> AtualizarUtilizador([FromBody] AtualizarUtilizadorRequest model)
        {
            if (model == null) return Json(new { sucesso = false, mensagem = "Dados inválidos." });

            var utilizador = await _db.UtilizadoresSistema.FindAsync(model.Id);
            if (utilizador == null) return Json(new { sucesso = false, mensagem = "Utilizador não encontrado." });

            utilizador.Email = model.Email;
            utilizador.IdPerfil = model.IdPerfil;
            utilizador.Estado = model.Estado;
            if (!string.IsNullOrWhiteSpace(model.Password))
                utilizador.PasswordHash = PasswordHelper.Hash(model.Password);

            await _db.SaveChangesAsync();

            return Json(new { sucesso = true, mensagem = "Utilizador actualizado com sucesso!" });
        }

        [HttpPost]
        public async Task<IActionResult> EliminarUtilizador(int id)
        {
            var utilizador = await _db.UtilizadoresSistema.FindAsync(id);
            if (utilizador == null) return Json(new { sucesso = false, mensagem = "Utilizador não encontrado." });

            _db.UtilizadoresSistema.Remove(utilizador);
            await _db.SaveChangesAsync();

            return Json(new { sucesso = true, mensagem = "Utilizador eliminado com sucesso!" });
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPermissoes([FromBody] GuardarPermissoesRequest model)
        {
            if (model == null) return Json(new { sucesso = false, mensagem = "Dados inválidos." });

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

            return Json(new { sucesso = true, mensagem = "Permissões actualizadas com sucesso!" });
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

        public IActionResult Auditoria()
        {
            ViewBag.ActivePage = "Auditoria";
            return View();
        }

        public IActionResult Configuracoes()
        {
            ViewBag.ActivePage = "Configuracoes";
            return View();
        }
    }
}
