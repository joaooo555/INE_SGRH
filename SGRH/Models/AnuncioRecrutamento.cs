using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("anuncio_recrutamento")]
public class AnuncioRecrutamento
{
    [Key]
    [Column("id_anuncio")]
    public int IdAnuncio { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Column("requisitos")]
    public string? Requisitos { get; set; }

    [Required]
    [Column("quantidade_vagas")]
    public int QuantidadeVagas { get; set; }

    [Required]
    [Column("data_publicacao")]
    public DateOnly DataPublicacao { get; set; }

    [Required]
    [Column("data_limite")]
    public DateOnly DataLimite { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Aberto";

    [Column("id_unidade_organica")]
    public int? IdUnidadeOrganica { get; set; }

    [Column("utilizador_criador")]
    public int? UtilizadorCriador { get; set; }

    [Column("data_registo")]
    public DateTime DataRegisto { get; set; } = DateTime.Now;

    // Navegação
    [ForeignKey("IdUnidadeOrganica")]
    public UnidadeOrganica? UnidadeOrganica { get; set; }

    public ICollection<Candidatura> Candidaturas { get; set; } = [];
}

