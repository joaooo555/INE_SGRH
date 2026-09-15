using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("actividade_estagio")]
public class ActividadeEstagio
{
    [Key]
    [Column("id_actividade")]
    public int IdActividade { get; set; }

    [Required]
    [Column("id_estagio")]
    public int IdEstagio { get; set; }

    [Required]
    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [Column("data_actividade")]
    public DateOnly DataActividade { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Planeada";

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    // Navegação
    [ForeignKey("IdEstagio")]
    public Estagio Estagio { get; set; } = null!;
}

