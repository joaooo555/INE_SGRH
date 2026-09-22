namespace SGRH.Models
{
    /// <summary>ViewModel do painel principal da Administração do Sistema.</summary>
    public class AdminDashboardViewModel
    {
        public int TotalUtilizadores { get; set; }
        public int UtilizadoresActivos { get; set; }
        public int UtilizadoresInactivos { get; set; }
        public int UtilizadoresBloqueados { get; set; }
        public int TotalPerfis { get; set; }
        public int AcessosHoje { get; set; }
        public int OperacoesHoje { get; set; }
        public List<LogAuditoria> UltimosAcessos { get; set; } = [];
    }
}
