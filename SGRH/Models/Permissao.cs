using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("permissao")]
public class Permissao
{
    [Key]
    [Column("id_permissao")]
    public int IdPermissao { get; set; }

    [Required]
    [Column("id_perfil")]
    public int IdPerfil { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("modulo")]
    public string Modulo { get; set; } = string.Empty;

    [Column("pode_visualizar")]
    public bool PodeVisualizar { get; set; } = true;

    [Column("pode_criar")]
    public bool PodeCriar { get; set; } = false;

    [Column("pode_editar")]
    public bool PodeEditar { get; set; } = false;

    [Column("pode_eliminar")]
    public bool PodeEliminar { get; set; } = false;

    // Navegação
    [ForeignKey("IdPerfil")]
    public PerfilAcesso PerfilAcesso { get; set; } = null!;
}

