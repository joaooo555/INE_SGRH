using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("documento")]
public class Documento
{
    [Key]
    [Column("id_documento")]
    public int IdDocumento { get; set; }

    [Required]
    [Column("id_colaborador")]
    public int IdColaborador { get; set; }

    [Required]
    [Column("id_tipo_documento")]
    public int IdTipoDocumento { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [Column("ficheiro")]
    public byte[] Ficheiro { get; set; } = [];

    [MaxLength(10)]
    [Column("formato")]
    public string? Formato { get; set; }

    [Column("data_upload")]
    public DateTime DataUpload { get; set; } = DateTime.Now;

    [Column("utilizador_upload")]
    public int? UtilizadorUpload { get; set; }

    // Navegação
    [ForeignKey("IdColaborador")]
    public Colaborador Colaborador { get; set; } = null!;

    [ForeignKey("IdTipoDocumento")]
    public TipoDocumento TipoDocumento { get; set; } = null!;

    [ForeignKey("UtilizadorUpload")]
    public UtilizadorSistema? Utilizador { get; set; }
}

