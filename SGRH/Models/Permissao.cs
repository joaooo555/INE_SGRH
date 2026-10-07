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

    /// <summary>
    /// Nível de acesso do perfil ao módulo, tal como definido na matriz de
    /// acessos (Total, Elevado, Consulta, Próprio, Limitado, Se supervisor,
    /// Restrito, Aprovação, Solicitação, Participação, Avaliação, Técnico,
    /// Consulta necessária, Consulta agregada, Próprios dados, Técnico restrito, Não).
    /// Os quatro booleanos abaixo são derivados deste nível para que a
    /// autorização existente (AutorizacaoService) continue a funcionar.
    /// </summary>
    [MaxLength(40)]
    [Column("nivel_acesso")]
    public string? NivelAcesso { get; set; }

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

