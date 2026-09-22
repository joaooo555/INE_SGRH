using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SGRH.Data;
using SGRH.Models;

namespace SGRH.Controllers
{

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

        /// <summary>
        /// Módulo 4 — Administração de Pessoal.
        /// Gestão de férias, faltas e licenças (registo, actualização, renovação, cancelamento)
        /// e workflow de parecer e aprovação, conforme o documento de requisitos.
        /// </summary>
        public async Task<IActionResult> Administracao()
        {
            ViewBag.ActivePage = "Administracao";

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
