using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SGRH.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.ActivePage = "Dashboard";
            return View();
        }

        public IActionResult Colaboradores()
        {
            ViewBag.ActivePage = "Colaboradores";
            return View();
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
