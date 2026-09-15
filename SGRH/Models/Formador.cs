using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("formador")]
public class Formador
{
    [Key]
    [Column("id_formador")]
    public int IdFormador { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("especialidade")]
    public string? Especialidade { get; set; }

    [MaxLength(100)]
    [Column("email")]
    public string? Email { get; set; }

    [MaxLength(15)]
    [Column("telefone")]
    public string? Telefone { get; set; }

    [MaxLength(100)]
    [Column("instituicao")]
    public string? Instituicao { get; set; }

    // Navegação
    public ICollection<AccaoFormador> AccaoFormadores { get; set; } = [];
}

