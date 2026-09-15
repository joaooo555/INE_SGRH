using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("avaliacao_formacao")]
public class AvaliacaoFormacao
{
    [Key]
    [Column("id_avaliacao")]
    public int IdAvaliacao { get; set; }

    [Required]
    [Column("id_inscricao")]
    public int IdInscricao { get; set; }

    [Required]
    [Column("data_avaliacao")]
    public DateOnly DataAvaliacao { get; set; }

    [Column("nota", TypeName = "decimal(4,2)")]
    public decimal? Nota { get; set; }

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    [Column("avaliador")]
    public int? Avaliador { get; set; }

    [Column("certificado")]
    public bool Certificado { get; set; } = false;

    // Navegação
    [ForeignKey("IdInscricao")]
    public InscricaoFormacao InscricaoFormacao { get; set; } = null!;
}

