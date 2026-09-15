using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("unidade_organica")]
public class UnidadeOrganica
{
    [Key]
    [Column("id_unidade_organica")]
    public int IdUnidadeOrganica { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(20)]
    [Column("sigla")]
    public string? Sigla { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("tipo")]
    public string Tipo { get; set; } = string.Empty;

    [Column("id_unidade_pai")]
    public int? IdUnidadePai { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Ativa";

    // Navegação — auto-referência
    [ForeignKey("IdUnidadePai")]
    public UnidadeOrganica? UnidadePai { get; set; }
    public ICollection<UnidadeOrganica> SubUnidades { get; set; } = [];

    // Navegação — dependentes
    public ICollection<Colaborador> Colaboradores { get; set; } = [];
    public ICollection<AnuncioRecrutamento> AnunciosRecrutamento { get; set; } = [];
    public ICollection<Estagio> Estagios { get; set; } = [];
    public ICollection<HistoricoColaborador> HistoricosColaborador { get; set; } = [];
}

