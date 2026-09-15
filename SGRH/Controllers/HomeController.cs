using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGRH.Data;
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

        public IActionResult Contratos()
        {
            ViewBag.ActivePage = "Contratos";
            return View();
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

        public IActionResult Utilizadores()
        {
            ViewBag.ActivePage = "Utilizadores";
            return View();
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
