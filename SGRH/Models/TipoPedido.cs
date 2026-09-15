using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("tipo_pedido")]
public class TipoPedido
{
    [Key]
    [Column("id_tipo_pedido")]
    public int IdTipoPedido { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    // Navegação
    public ICollection<PedidoAprovacao> PedidosAprovacao { get; set; } = [];
}

