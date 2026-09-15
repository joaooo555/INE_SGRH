using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("carreira")]
public class Carreira
{
    [Key]
    [Column("id_carreira")]
    public int IdCarreira { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("descricao")]
    public string? Descricao { get; set; }

    [MaxLength(50)]
    [Column("classe_inicial")]
    public string? ClasseInicial { get; set; }

    [MaxLength(50)]
    [Column("classe_final")]
    public string? ClasseFinal { get; set; }

    // Navegação
    public ICollection<Colaborador> Colaboradores { get; set; } = [];
    public ICollection<HistoricoColaborador> HistoricosColaborador { get; set; } = [];
}

