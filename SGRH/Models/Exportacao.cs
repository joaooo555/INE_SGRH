using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("exportacao")]
public class Exportacao
{
    [Key]
    [Column("id_exportacao")]
    public int IdExportacao { get; set; }

    [Required]
    [Column("id_utilizador")]
    public int IdUtilizador { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("tipo_exportacao")]
    public string TipoExportacao { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("modulo_origem")]
    public string ModuloOrigem { get; set; } = string.Empty;

    [Column("filtros_aplicados")]
    public string? FiltrosAplicados { get; set; }

    [Column("data_exportacao")]
    public DateTime DataExportacao { get; set; } = DateTime.Now;

    [MaxLength(255)]
    [Column("ficheiro_gerado")]
    public string? FicheiroGerado { get; set; }

    // Navegação
    [ForeignKey("IdUtilizador")]
    public UtilizadorSistema UtilizadorSistema { get; set; } = null!;
}

