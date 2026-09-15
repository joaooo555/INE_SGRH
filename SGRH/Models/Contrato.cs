using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("contrato")]
public class Contrato
{
    [Key]
    [Column("id_contrato")]
    public int IdContrato { get; set; }

    [Required]
    [Column("id_colaborador")]
    public int IdColaborador { get; set; }

    [Required]
    [Column("id_tipo_contrato")]
    public int IdTipoContrato { get; set; }

    [MaxLength(30)]
    [Column("numero_contrato")]
    public string? NumeroContrato { get; set; }

    [Required]
    [Column("data_inicio")]
    public DateOnly DataInicio { get; set; }

    [Column("data_fim")]
    public DateOnly? DataFim { get; set; }

    [Column("objecto")]
    public string? Objecto { get; set; }

    [Column("remuneracao", TypeName = "decimal(10,2)")]
    public decimal? Remuneracao { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Vigente";

    [Column("documento_contrato")]
    public byte[]? DocumentoContrato { get; set; }

    [Column("data_assinatura")]
    public DateOnly? DataAssinatura { get; set; }

    [Column("renovacao_anterior")]
    public int? RenovacaoAnterior { get; set; }

    [Column("utilizador_registo")]
    public int? UtilizadorRegisto { get; set; }

    [Column("data_registo")]
    public DateTime DataRegisto { get; set; } = DateTime.Now;

    // Navegação
    [ForeignKey("IdColaborador")]
    public Colaborador Colaborador { get; set; } = null!;

    [ForeignKey("IdTipoContrato")]
    public TipoContrato TipoContrato { get; set; } = null!;

    [ForeignKey("RenovacaoAnterior")]
    public Contrato? ContratoAnterior { get; set; }

    public ICollection<Contrato> Renovacoes { get; set; } = [];
}

