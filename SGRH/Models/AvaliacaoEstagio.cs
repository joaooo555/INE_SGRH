using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("avaliacao_estagio")]
public class AvaliacaoEstagio
{
    [Key]
    [Column("id_avaliacao")]
    public int IdAvaliacao { get; set; }

    [Required]
    [Column("id_estagio")]
    public int IdEstagio { get; set; }

    [Required]
    [Column("data_avaliacao")]
    public DateOnly DataAvaliacao { get; set; }

    [Required]
    [MaxLength(30)]
    [Column("periodo")]
    public string Periodo { get; set; } = string.Empty;

    [Column("nota", TypeName = "decimal(4,2)")]
    public decimal? Nota { get; set; }

    [Column("pontos_fortes")]
    public string? PontosFortes { get; set; }

    [Column("pontos_melhoria")]
    public string? PontosMelhoria { get; set; }

    [Column("avaliador")]
    public int? Avaliador { get; set; }

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    // Navegação
    [ForeignKey("IdEstagio")]
    public Estagio Estagio { get; set; } = null!;
}

