using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

/// <summary>
/// Tabela de ligação Many-to-Many entre AccaoFormacao e Formador.
/// Chave primária composta configurada via Fluent API no DbContext.
/// </summary>
[Table("accao_formador")]
public class AccaoFormador
{
    [Required]
    [Column("id_accao")]
    public int IdAccao { get; set; }

    [Required]
    [Column("id_formador")]
    public int IdFormador { get; set; }

    // Navegação
    [ForeignKey("IdAccao")]
    public AccaoFormacao AccaoFormacao { get; set; } = null!;

    [ForeignKey("IdFormador")]
    public Formador Formador { get; set; } = null!;
}

