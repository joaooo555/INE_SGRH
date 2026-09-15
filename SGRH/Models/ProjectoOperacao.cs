using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("projecto_operacao")]
public class ProjectoOperacao
{
    [Key]
    [Column("id_projecto")]
    public int IdProjecto { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("data_inicio")]
    public DateOnly? DataInicio { get; set; }

    [Column("data_fim")]
    public DateOnly? DataFim { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Planeado";

    // Navegação
    public ICollection<GuiaMarcha> GuiasMarcha { get; set; } = [];
}

