using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("pedido_aprovacao")]
public class PedidoAprovacao
{
    [Key]
    [Column("id_pedido")]
    public int IdPedido { get; set; }

    [Required]
    [Column("id_colaborador_solicitante")]
    public int IdColaboradorSolicitante { get; set; }

    [Required]
    [Column("id_tipo_pedido")]
    public int IdTipoPedido { get; set; }

    [Required]
    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Column("data_submissao")]
    public DateTime DataSubmissao { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Pendente";

    // Navegação
    [ForeignKey("IdColaboradorSolicitante")]
    public Colaborador Colaborador { get; set; } = null!;

    [ForeignKey("IdTipoPedido")]
    public TipoPedido TipoPedido { get; set; } = null!;

    public ICollection<AprovacaoPedido> Aprovacoes { get; set; } = [];
}

