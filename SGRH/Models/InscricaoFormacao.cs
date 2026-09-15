using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("inscricao_formacao")]
public class InscricaoFormacao
{
    [Key]
    [Column("id_inscricao")]
    public int IdInscricao { get; set; }

    [Required]
    [Column("id_accao")]
    public int IdAccao { get; set; }

    [Required]
    [Column("id_colaborador")]
    public int IdColaborador { get; set; }

    [Column("data_inscricao")]
    public DateTime DataInscricao { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Inscrito";

    // Navegação
    [ForeignKey("IdAccao")]
    public AccaoFormacao AccaoFormacao { get; set; } = null!;

    [ForeignKey("IdColaborador")]
    public Colaborador Colaborador { get; set; } = null!;

    public AvaliacaoFormacao? AvaliacaoFormacao { get; set; }
}

