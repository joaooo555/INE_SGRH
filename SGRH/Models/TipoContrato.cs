using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("tipo_contrato")]
public class TipoContrato
{
    [Key]
    [Column("id_tipo_contrato")]
    public int IdTipoContrato { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("duracao_maxima_meses")]
    public int? DuracaoMaximaMeses { get; set; }

    // Navegação
    public ICollection<Contrato> Contratos { get; set; } = [];
}

