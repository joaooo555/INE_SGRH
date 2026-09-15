using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("aprovacao_pedido")]
public class AprovacaoPedido
{
    [Key]
    [Column("id_aprovacao")]
    public int IdAprovacao { get; set; }

    [Required]
    [Column("id_pedido")]
    public int IdPedido { get; set; }

    [Required]
    [Column("id_aprovador")]
    public int IdAprovador { get; set; }

    [Required]
    [Column("nivel_aprovacao")]
    public int NivelAprovacao { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("decisao")]
    public string Decisao { get; set; } = string.Empty;

    [Column("parecer")]
    public string? Parecer { get; set; }

    [Column("data_decisao")]
    public DateTime DataDecisao { get; set; } = DateTime.Now;

    // Navegação
    [ForeignKey("IdPedido")]
    public PedidoAprovacao PedidoAprovacao { get; set; } = null!;

    [ForeignKey("IdAprovador")]
    public Colaborador Aprovador { get; set; } = null!;
}

