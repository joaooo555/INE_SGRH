using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("historico_colaborador")]
public class HistoricoColaborador
{
    [Key]
    [Column("id_historico")]
    public int IdHistorico { get; set; }

    [Required]
    [Column("id_colaborador")]
    public int IdColaborador { get; set; }

    [Required]
    [Column("data_evento")]
    public DateOnly DataEvento { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("tipo_evento")]
    public string TipoEvento { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }

    [MaxLength(100)]
    [Column("referencia")]
    public string? Referencia { get; set; }

    [Column("id_unidade_organica")]
    public int? IdUnidadeOrganica { get; set; }

    [Column("id_categoria")]
    public int? IdCategoria { get; set; }

    [Column("id_carreira")]
    public int? IdCarreira { get; set; }

    [Column("id_funcao")]
    public int? IdFuncao { get; set; }

    // Navegação
    [ForeignKey("IdColaborador")]
    public Colaborador Colaborador { get; set; } = null!;

    [ForeignKey("IdUnidadeOrganica")]
    public UnidadeOrganica? UnidadeOrganica { get; set; }

    [ForeignKey("IdCategoria")]
    public Categoria? Categoria { get; set; }

    [ForeignKey("IdCarreira")]
    public Carreira? Carreira { get; set; }

    [ForeignKey("IdFuncao")]
    public Funcao? Funcao { get; set; }
}

