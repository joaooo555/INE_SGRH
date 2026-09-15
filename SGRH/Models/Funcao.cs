using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("funcao")]
public class Funcao
{
    [Key]
    [Column("id_funcao")]
    public int IdFuncao { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("remuneracao_base", TypeName = "decimal(10,2)")]
    public decimal? RemuneracaoBase { get; set; }

    // Navegação
    public ICollection<Colaborador> Colaboradores { get; set; } = [];
    public ICollection<HistoricoColaborador> HistoricosColaborador { get; set; } = [];
}

