using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("acto_administrativo")]
public class ActoAdministrativo
{
    [Key]
    [Column("id_acto")]
    public int IdActo { get; set; }

    [Required]
    [Column("id_colaborador")]
    public int IdColaborador { get; set; }

    [Required]
    [Column("id_tipo_acto")]
    public int IdTipoActo { get; set; }

    [Required]
    [Column("data_acto")]
    public DateOnly DataActo { get; set; }

    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("documento_suporte")]
    public byte[]? DocumentoSuporte { get; set; }

    [Column("utilizador_registo")]
    public int? UtilizadorRegisto { get; set; }

    [Column("data_registo")]
    public DateTime DataRegisto { get; set; } = DateTime.Now;

    // Navegação
    [ForeignKey("IdColaborador")]
    public Colaborador Colaborador { get; set; } = null!;

    [ForeignKey("IdTipoActo")]
    public TipoActo TipoActo { get; set; } = null!;

    [ForeignKey("UtilizadorRegisto")]
    public UtilizadorSistema? Utilizador { get; set; }
}

