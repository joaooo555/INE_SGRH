using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("log_auditoria")]
public class LogAuditoria
{
    [Key]
    [Column("id_log")]
    public int IdLog { get; set; }

    [Column("id_colaborador")]
    public int? IdColaborador { get; set; }

    [Column("id_utilizador")]
    public int? IdUtilizador { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("tabela_afetada")]
    public string TabelaAfetada { get; set; } = string.Empty;  // coluna que faltava no SQL original

    [Required]
    [MaxLength(20)]
    [Column("operacao")]
    public string Operacao { get; set; } = string.Empty;

    [Column("dados_anteriores")]
    public string? DadosAnteriores { get; set; }

    [Column("dados_posteriores")]
    public string? DadosPosteriores { get; set; }

    [Column("data_hora")]
    public DateTime DataHora { get; set; } = DateTime.Now;

    [MaxLength(45)]
    [Column("ip_address")]
    public string? IpAddress { get; set; }

    // Navegação
    [ForeignKey("IdColaborador")]
    public Colaborador? Colaborador { get; set; }

    [ForeignKey("IdUtilizador")]
    public UtilizadorSistema? UtilizadorSistema { get; set; }
}

