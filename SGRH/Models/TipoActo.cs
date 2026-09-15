using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("tipo_acto")]
public class TipoActo
{
    [Key]
    [Column("id_tipo_acto")]
    public int IdTipoActo { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    // Navegação
    public ICollection<ActoAdministrativo> ActosAdministrativos { get; set; } = [];
}

