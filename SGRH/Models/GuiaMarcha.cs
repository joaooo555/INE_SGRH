using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("guia_marcha")]
public class GuiaMarcha
{
    [Key]
    [Column("id_guia")]
    public int IdGuia { get; set; }

    [Required]
    [Column("id_funcionario")]
    public int IdFuncionario { get; set; }

    [Column("id_projecto")]
    public int? IdProjecto { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("destino")]
    public string Destino { get; set; } = string.Empty;

    [Required]
    [Column("data_partida")]
    public DateOnly DataPartida { get; set; }

    [Column("data_chegada")]
    public DateOnly? DataChegada { get; set; }

    [Required]
    [Column("missao")]
    public string Missao { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Programada";

    [Column("autorizacao_superior")]
    public int? AutorizacaoSuperior { get; set; }

    [Column("utilizador_registo")]
    public int? UtilizadorRegisto { get; set; }

    [Column("data_registo")]
    public DateTime DataRegisto { get; set; } = DateTime.Now;

    // Navegação
    [ForeignKey("IdFuncionario")]
    public Colaborador Funcionario { get; set; } = null!;

    [ForeignKey("IdProjecto")]
    public ProjectoOperacao? ProjectoOperacao { get; set; }

    [ForeignKey("AutorizacaoSuperior")]
    public Colaborador? Superior { get; set; }
}

