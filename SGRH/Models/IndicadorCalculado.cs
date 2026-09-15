using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("indicador_calculado")]
public class IndicadorCalculado
{
    [Key]
    [Column("id_indicador")]
    public int IdIndicador { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("tipo")]
    public string Tipo { get; set; } = string.Empty;

    [Column("formula")]
    public string? Formula { get; set; }

    [MaxLength(30)]
    [Column("periodo_referencia")]
    public string? PeriodoReferencia { get; set; }

    [Column("valor_calculado", TypeName = "decimal(10,4)")]
    public decimal? ValorCalculado { get; set; }

    [Column("data_calculo")]
    public DateTime DataCalculo { get; set; } = DateTime.Now;
}

