using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

/// <summary>
/// Violação 3FN corrigida: removido fase_actual (atributo derivado de fase_processo).
/// Para obter a fase atual: FasesProcesso.OrderByDescending(f => f.DataInicio).First()
/// </summary>
[Table("candidatura")]
public class Candidatura
{
    [Key]
    [Column("id_candidatura")]
    public int IdCandidatura { get; set; }

    [Required]
    [Column("id_candidato")]
    public int IdCandidato { get; set; }

    [Required]
    [Column("id_anuncio")]
    public int IdAnuncio { get; set; }

    [Column("data_submissao")]
    public DateTime DataSubmissao { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(30)]
    [Column("estado")]
    public string Estado { get; set; } = "Submetida";

    [Column("motivo_nao_selecao")]
    public string? MotivoNaoSelecao { get; set; }

    // Navegação
    [ForeignKey("IdCandidato")]
    public Candidato Candidato { get; set; } = null!;

    [ForeignKey("IdAnuncio")]
    public AnuncioRecrutamento AnuncioRecrutamento { get; set; } = null!;

    public ICollection<FaseProcesso> FasesProcesso { get; set; } = [];
    public ICollection<ComunicacaoCandidato> Comunicacoes { get; set; } = [];
}
