using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("estagio")]
public class Estagio
{
    [Key]
    [Column("id_estagio")]
    public int IdEstagio { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("nome_estagiario")]
    public string NomeEstagiario { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("email")]
    public string? Email { get; set; }

    [MaxLength(15)]
    [Column("telefone")]
    public string? Telefone { get; set; }

    [MaxLength(30)]
    [Column("documento_identificacao")]
    public string? DocumentoIdentificacao { get; set; }

    [MaxLength(150)]
    [Column("instituicao_ensino")]
    public string? InstituicaoEnsino { get; set; }

    [Required]
    [MaxLength(30)]
    [Column("tipo_estagio")]
    public string TipoEstagio { get; set; } = string.Empty;

    [Required]
    [Column("id_unidade_organica")]
    public int IdUnidadeOrganica { get; set; }

    [Required]
    [Column("id_supervisor")]
    public int IdSupervisor { get; set; }

    [Required]
    [Column("data_inicio")]
    public DateOnly DataInicio { get; set; }

    [Column("data_fim")]
    public DateOnly? DataFim { get; set; }

    [Column("plano_estagio")]
    public string? PlanoEstagio { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Solicitado";

    [Column("declaracao_emitida")]
    public bool DeclaracaoEmitida { get; set; } = false;

    [Column("utilizador_registo")]
    public int? UtilizadorRegisto { get; set; }

    [Column("data_registo")]
    public DateTime DataRegisto { get; set; } = DateTime.Now;

    // Navegação
    [ForeignKey("IdUnidadeOrganica")]
    public UnidadeOrganica UnidadeOrganica { get; set; } = null!;

    [ForeignKey("IdSupervisor")]
    public Colaborador Supervisor { get; set; } = null!;

    public ICollection<AvaliacaoEstagio> Avaliacoes { get; set; } = [];
    public ICollection<ActividadeEstagio> Actividades { get; set; } = [];
}

