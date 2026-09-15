using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("colaborador")]
public class Colaborador
{
    [Key]
    [Column("id_colaborador")]
    public int IdColaborador { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("nome_completo")]
    public string NomeCompleto { get; set; } = string.Empty;

    [MaxLength(14)]
    [Column("nuit")]
    public string? Nuit { get; set; }

    [Required]
    [Column("data_nascimento")]
    public DateOnly DataNascimento { get; set; }

    [Required]
    [MaxLength(1)]
    [Column("sexo")]
    public string Sexo { get; set; } = string.Empty;

    [Column("id_estado_civil")]
    public int? IdEstadoCivil { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nacionalidade")]
    public string Nacionalidade { get; set; } = "Moçambicana";

    [MaxLength(30)]
    [Column("numero_identificacao")]
    public string? NumeroIdentificacao { get; set; }

    [MaxLength(15)]
    [Column("contacto_telefonico")]
    public string? ContactoTelefonico { get; set; }

    [MaxLength(100)]
    [Column("contacto_email")]
    public string? ContactoEmail { get; set; }

    [MaxLength(200)]
    [Column("endereco_residencia")]
    public string? EnderecoResidencia { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Ativo";

    [Column("id_unidade_organica")]
    public int? IdUnidadeOrganica { get; set; }

    [Column("id_categoria")]
    public int? IdCategoria { get; set; }

    [Column("id_carreira")]
    public int? IdCarreira { get; set; }

    [Column("id_funcao")]
    public int? IdFuncao { get; set; }

    [Required]
    [Column("data_ingresso")]
    public DateOnly DataIngresso { get; set; }

    [Column("id_forma_ingresso")]
    public int? IdFormaIngresso { get; set; }

    [MaxLength(255)]
    [Column("foto_path")]
    public string? FotoPath { get; set; }

    [MaxLength(500)]
    [Column("observacoes")]
    public string? Observacoes { get; set; }

    [Column("data_registo")]
    public DateTime DataRegisto { get; set; } = DateTime.Now;

    // Navegação — FKs
    [ForeignKey("IdEstadoCivil")]
    public EstadoCivil? EstadoCivil { get; set; }

    [ForeignKey("IdUnidadeOrganica")]
    public UnidadeOrganica? UnidadeOrganica { get; set; }

    [ForeignKey("IdCategoria")]
    public Categoria? Categoria { get; set; }

    [ForeignKey("IdCarreira")]
    public Carreira? Carreira { get; set; }

    [ForeignKey("IdFuncao")]
    public Funcao? Funcao { get; set; }

    [ForeignKey("IdFormaIngresso")]
    public FormaIngresso? FormaIngresso { get; set; }

    // Navegação — coleções
    public ICollection<ActoAdministrativo> ActosAdministrativos { get; set; } = [];
    public ICollection<Documento> Documentos { get; set; } = [];
    public ICollection<HistoricoColaborador> HistoricosColaborador { get; set; } = [];
    public ICollection<Contrato> Contratos { get; set; } = [];
    public ICollection<RegistoAusencia> RegistosAusencia { get; set; } = [];
    public ICollection<PedidoAprovacao> PedidosAprovacao { get; set; } = [];
    public ICollection<AprovacaoPedido> AprovacoesPedido { get; set; } = [];
    public ICollection<InscricaoFormacao> InscricoesFormacao { get; set; } = [];
    public ICollection<Estagio> EstagiosSupervisados { get; set; } = [];
    public ICollection<GuiaMarcha> GuiasMarcha { get; set; } = [];
    public ICollection<GuiaMarcha> GuiasMarchaAutorizadas { get; set; } = [];
    public ICollection<CasoApoioSocial> CasosApoioSocial { get; set; } = [];
    public ICollection<LogAuditoria> LogsAuditoria { get; set; } = [];
    public UtilizadorSistema? UtilizadorSistema { get; set; }
}

