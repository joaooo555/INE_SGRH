using System.Diagnostics;
using System.Text.Json;
using SGRH.Data;
using SGRH.Models;

namespace SGRH.Services
{
    /// <summary>
    /// RT08 — Auditoria / Logs: todas as operações relevantes devem ser registadas em logs.
    /// Também concretiza o "sistema de Logs - Olheiro" descrito na secção 12.1 do documento
    /// (último passo do fluxo: "Registo da operação no sistema").
    /// </summary>
    public interface IAuditoriaService
    {
        Task RegistarAsync(int? idUtilizador, string tabela, string operacao,
            object? dadosAnteriores, object? dadosPosteriores, int? idColaborador = null);
    }

    public class AuditoriaService : IAuditoriaService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _http;

        public AuditoriaService(AppDbContext db, IHttpContextAccessor http)
        {
            _db = db;
            _http = http;
        }

        public async Task RegistarAsync(int? idUtilizador, string tabela, string operacao,
            object? dadosAnteriores, object? dadosPosteriores, int? idColaborador = null)
        {
            try
            {
                var ip = _http.HttpContext?.Connection.RemoteIpAddress?.ToString();

                _db.LogsAuditoria.Add(new LogAuditoria
                {
                    IdUtilizador = idUtilizador,
                    IdColaborador = idColaborador,
                    TabelaAfetada = tabela,
                    Operacao = operacao,
                    DadosAnteriores = dadosAnteriores == null ? null : JsonSerializer.Serialize(dadosAnteriores),
                    DadosPosteriores = dadosPosteriores == null ? null : JsonSerializer.Serialize(dadosPosteriores),
                    DataHora = DateTime.Now,
                    IpAddress = ip == null ? null : ip.Length > 45 ? ip[..45] : ip
                });
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // A auditoria nunca deve quebrar a operação principal — regista e segue.
                Debug.WriteLine("Falha ao registar log de auditoria: " + ex.Message);
            }
        }
    }
}
