using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("forma_ingresso")]
public class FormaIngresso
{
    [Key]
    [Column("id_forma_ingresso")]
    public int IdFormaIngresso { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    // Navegação
    public ICollection<Colaborador> Colaboradores { get; set; } = [];
}

