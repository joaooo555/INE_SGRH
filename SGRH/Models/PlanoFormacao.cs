using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("plano_formacao")]
public class PlanoFormacao
{
    [Key]
    [Column("id_plano")]
    public int IdPlano { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("designacao")]
    public string Designacao { get; set; } = string.Empty;

    [Required]
    [Column("ano")]
    public int Ano { get; set; }

    [Column("data_inicio")]
    public DateOnly? DataInicio { get; set; }

    [Column("data_fim")]
    public DateOnly? DataFim { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Rascunho";

    [Column("utilizador_criador")]
    public int? UtilizadorCriador { get; set; }

    [Column("data_registo")]
    public DateTime DataRegisto { get; set; } = DateTime.Now;

    // Navegação
    public ICollection<AccaoFormacao> AccoesFormacao { get; set; } = [];
}

