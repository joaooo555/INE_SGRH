using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("tipo_documento")]
public class TipoDocumento
{
    [Key]
    [Column("id_tipo_documento")]
    public int IdTipoDocumento { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    // Navegação
    public ICollection<Documento> Documentos { get; set; } = [];
}

