using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("utilizador_sistema")]
public class UtilizadorSistema
{
    [Key]
    [Column("id_utilizador")]
    public int IdUtilizador { get; set; }

    [Column("id_colaborador")]
    public int? IdColaborador { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("id_perfil")]
    public int IdPerfil { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Ativo";

    [Column("ultimo_acesso")]
    public DateTime? UltimoAcesso { get; set; }

    [Column("data_criacao")]
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    // Navegação
    [ForeignKey("IdColaborador")]
    public Colaborador? Colaborador { get; set; }

    [ForeignKey("IdPerfil")]
    public PerfilAcesso PerfilAcesso { get; set; } = null!;

    public ICollection<LogAuditoria> LogsAuditoria { get; set; } = [];
    public ICollection<Exportacao> Exportacoes { get; set; } = [];
}

