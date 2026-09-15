using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("registo_ausencia")]
public class RegistoAusencia
{
    [Key]
    [Column("id_registo")]
    public int IdRegisto { get; set; }

    [Required]
    [Column("id_colaborador")]
    public int IdColaborador { get; set; }

    [Required]
    [Column("id_tipo_ausencia")]
    public int IdTipoAusencia { get; set; }

    [Required]
    [Column("data_inicio")]
    public DateOnly DataInicio { get; set; }

    [Required]
    [Column("data_fim")]
    public DateOnly DataFim { get; set; }

    [Column("motivo")]
    public string? Motivo { get; set; }

    [Column("documento_suporte")]
    public byte[]? DocumentoSuporte { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Pendente";

    [Column("utilizador_registo")]
    public int? UtilizadorRegisto { get; set; }

    [Column("data_registo")]
    public DateTime DataRegisto { get; set; } = DateTime.Now;

    // Navegação
    [ForeignKey("IdColaborador")]
    public Colaborador Colaborador { get; set; } = null!;

    [ForeignKey("IdTipoAusencia")]
    public TipoAusencia TipoAusencia { get; set; } = null!;
}

