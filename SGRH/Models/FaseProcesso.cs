using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("fase_processo")]
public class FaseProcesso
{
    [Key]
    [Column("id_fase")]
    public int IdFase { get; set; }

    [Required]
    [Column("id_candidatura")]
    public int IdCandidatura { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nome_fase")]
    public string NomeFase { get; set; } = string.Empty;

    [Column("data_inicio")]
    public DateOnly? DataInicio { get; set; }

    [Column("data_fim")]
    public DateOnly? DataFim { get; set; }

    [MaxLength(30)]
    [Column("resultado")]
    public string? Resultado { get; set; }

    [Column("avaliador")]
    public int? Avaliador { get; set; }

    [Column("nota", TypeName = "decimal(4,2)")]
    public decimal? Nota { get; set; }

    [Column("observacoes")]
    public string? Observacoes { get; set; }

    // Navegação
    [ForeignKey("IdCandidatura")]
    public Candidatura Candidatura { get; set; } = null!;
}

