using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("nota_acompanhamento")]
public class NotaAcompanhamento
{
    [Key]
    [Column("id_nota")]
    public int IdNota { get; set; }

    [Required]
    [Column("id_caso")]
    public int IdCaso { get; set; }

    [Column("data_nota")]
    public DateTime DataNota { get; set; } = DateTime.Now;

    [Required]
    [Column("conteudo")]
    public string Conteudo { get; set; } = string.Empty;

    [Column("autor")]
    public int? Autor { get; set; }

    // Navegação
    [ForeignKey("IdCaso")]
    public CasoApoioSocial CasoApoioSocial { get; set; } = null!;
}

