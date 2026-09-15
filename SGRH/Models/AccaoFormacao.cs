using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("accao_formacao")]
public class AccaoFormacao
{
    [Key]
    [Column("id_accao")]
    public int IdAccao { get; set; }

    [Column("id_plano")]
    public int? IdPlano { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }

    [MaxLength(100)]
    [Column("tema")]
    public string? Tema { get; set; }

    [Required]
    [Column("data_inicio")]
    public DateOnly DataInicio { get; set; }

    [Column("data_fim")]
    public DateOnly? DataFim { get; set; }

    [MaxLength(150)]
    [Column("local")]
    public string? Local { get; set; }

    [Column("vagas_disponiveis")]
    public int? VagasDisponiveis { get; set; }

    [Column("horas", TypeName = "decimal(5,1)")]
    public decimal? Horas { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Planeada";

    // Navegação
    [ForeignKey("IdPlano")]
    public PlanoFormacao? PlanoFormacao { get; set; }

    public ICollection<AccaoFormador> AccaoFormadores { get; set; } = [];
    public ICollection<InscricaoFormacao> Inscricoes { get; set; } = [];
}

