using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("estado_civil")]
public class EstadoCivil
{
    [Key]
    [Column("id_estado_civil")]
    public int IdEstadoCivil { get; set; }

    [Required]
    [MaxLength(30)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    // Navegação
    public ICollection<Colaborador> Colaboradores { get; set; } = [];
}

