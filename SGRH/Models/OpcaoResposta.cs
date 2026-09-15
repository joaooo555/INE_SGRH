using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

/// <summary>
/// Nova tabela criada para normalizar pergunta_inquerito.opcoes_resposta (1FN).
/// Cada linha representa uma opção de resposta para perguntas de escolha múltipla.
/// </summary>
[Table("opcao_resposta")]
public class OpcaoResposta
{
    [Key]
    [Column("id_opcao")]
    public int IdOpcao { get; set; }

    [Required]
    [Column("id_pergunta")]
    public int IdPergunta { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("texto_opcao")]
    public string TextoOpcao { get; set; } = string.Empty;

    [Required]
    [Column("ordem")]
    public int Ordem { get; set; }

    // Navegação
    [ForeignKey("IdPergunta")]
    public PerguntaInquerito PerguntaInquerito { get; set; } = null!;
}

