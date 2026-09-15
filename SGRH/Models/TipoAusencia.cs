using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("tipo_ausencia")]
public class TipoAusencia
{
    [Key]
    [Column("id_tipo_ausencia")]
    public int IdTipoAusencia { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("dias_maximos")]
    public int? DiasMaximos { get; set; }

    // Navegação
    public ICollection<RegistoAusencia> RegistosAusencia { get; set; } = [];
}

