using SGRH.Models;

namespace SGRH.Models
{
    /// <summary>ViewModel do Módulo 4 — Administração de Pessoal.</summary>
    public class AdministracaoPessoalViewModel
    {
        public List<RegistoAusencia> RegistosAusencia { get; set; } = [];
        public List<PedidoAprovacao> PedidosAprovacao { get; set; } = [];
        public List<Colaborador> Colaboradores { get; set; } = [];
        public List<TipoAusencia> TiposAusencia { get; set; } = [];
        public List<TipoPedido> TiposPedido { get; set; } = [];
    }
}
