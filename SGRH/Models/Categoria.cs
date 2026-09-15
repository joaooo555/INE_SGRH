using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("categoria")]
public class Categoria
{
    [Key]
    [Column("id_categoria")]
    public int IdCategoria { get; set; }

    [MaxLength(10)]
    [Column("codigo")]
    public string? Codigo { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Column("nivel")]
    public int? Nivel { get; set; }

    // Navegação
    public ICollection<Colaborador> Colaboradores { get; set; } = [];
    public ICollection<HistoricoColaborador> HistoricosColaborador { get; set; } = [];
}

