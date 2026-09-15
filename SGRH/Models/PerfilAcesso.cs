using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("perfil_acesso")]
public class PerfilAcesso
{
    [Key]
    [Column("id_perfil")]
    public int IdPerfil { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(200)]
    [Column("descricao")]
    public string? Descricao { get; set; }

    [Required]
    [Column("nivel_confidencialidade")]
    public int NivelConfidencialidade { get; set; } = 1;

    // Navegação
    public ICollection<Permissao> Permissoes { get; set; } = [];
    public ICollection<UtilizadorSistema> Utilizadores { get; set; } = [];
}

