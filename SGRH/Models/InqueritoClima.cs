using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("inquerito_clima")]
public class InqueritoClima
{
    [Key]
    [Column("id_inquerito")]
    public int IdInquerito { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }

    [Required]
    [Column("data_inicio")]
    public DateOnly DataInicio { get; set; }

    [Column("data_fim")]
    public DateOnly? DataFim { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Rascunho";

    [Column("anonimato_garantido")]
    public bool AnonimatorGarantido { get; set; } = true;

    [Column("utilizador_criador")]
    public int? UtilizadorCriador { get; set; }

    [Column("data_registo")]
    public DateTime DataRegisto { get; set; } = DateTime.Now;

    // Navegação
    public ICollection<PerguntaInquerito> Perguntas { get; set; } = [];
    public ICollection<RespostaInquerito> Respostas { get; set; } = [];
}

