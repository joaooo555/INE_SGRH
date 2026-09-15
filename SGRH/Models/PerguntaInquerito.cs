using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

/// <summary>
/// Violação 1FN corrigida: removido opcoes_resposta (coluna não atómica).
/// Opções movidas para tabela OpcaoResposta (1-para-muitos).
/// </summary>
[Table("pergunta_inquerito")]
public class PerguntaInquerito
{
    [Key]
    [Column("id_pergunta")]
    public int IdPergunta { get; set; }

    [Required]
    [Column("id_inquerito")]
    public int IdInquerito { get; set; }

    [Required]
    [Column("texto_pergunta")]
    public string TextoPergunta { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("tipo_pergunta")]
    public string TipoPergunta { get; set; } = string.Empty;

    [Required]
    [Column("ordem")]
    public int Ordem { get; set; }

    [Column("obrigatoria")]
    public bool Obrigatoria { get; set; } = true;

    // Navegação
    [ForeignKey("IdInquerito")]
    public InqueritoClima InqueritoClima { get; set; } = null!;

    public ICollection<OpcaoResposta> OpcoesResposta { get; set; } = [];  // 1FN fix
    public ICollection<RespostaInquerito> Respostas { get; set; } = [];
}
