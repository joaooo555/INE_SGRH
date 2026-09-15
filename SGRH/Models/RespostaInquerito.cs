using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

/// <summary>
/// Violação 3FN corrigida: removido id_inquerito (dependência transitiva via id_pergunta).
/// Para obter o inquérito: RespostaInquerito.PerguntaInquerito.InqueritoClima
/// </summary>
[Table("resposta_inquerito")]
public class RespostaInquerito
{
    [Key]
    [Column("id_resposta")]
    public int IdResposta { get; set; }

    [Required]
    [Column("id_pergunta")]
    public int IdPergunta { get; set; }

    [Required]
    [MaxLength(64)]
    [Column("hash_respondente")]
    public string HashRespondente { get; set; } = string.Empty;

    [Column("resposta_texto")]
    public string? RespostaTexto { get; set; }

    [Column("resposta_numerica")]
    public int? RespostaNumerica { get; set; }

    [Column("data_resposta")]
    public DateTime DataResposta { get; set; } = DateTime.Now;

    // Navegação
    [ForeignKey("IdPergunta")]
    public PerguntaInquerito PerguntaInquerito { get; set; } = null!;
}
