using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

/// <summary>
/// Violação 3FN corrigida: removido data_registo (redundante — derivável de data_criacao).
/// Para obter apenas a data: DateOnly.FromDateTime(DataCriacao)
/// </summary>
[Table("caso_apoio_social")]
public class CasoApoioSocial
{
    [Key]
    [Column("id_caso")]
    public int IdCaso { get; set; }

    [Column("id_colaborador")]
    public int? IdColaborador { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("tipo_caso")]
    public string TipoCaso { get; set; } = string.Empty;

    [Required]
    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Registado";

    [Required]
    [MaxLength(30)]
    [Column("canal_origem")]
    public string CanalOrigem { get; set; } = string.Empty;

    [Column("documento_suporte")]
    public byte[]? DocumentoSuporte { get; set; }

    [Column("utilizador_registo")]
    public int? UtilizadorRegisto { get; set; }

    [Column("data_criacao")]
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    // Navegação
    [ForeignKey("IdColaborador")]
    public Colaborador? Colaborador { get; set; }

    public ICollection<NotaAcompanhamento> Notas { get; set; } = [];
}
