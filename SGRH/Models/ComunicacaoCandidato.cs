using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("comunicacao_candidato")]
public class ComunicacaoCandidato
{
    [Key]
    [Column("id_comunicacao")]
    public int IdComunicacao { get; set; }

    [Required]
    [Column("id_candidatura")]
    public int IdCandidatura { get; set; }

    [Required]
    [MaxLength(30)]
    [Column("tipo_comunicacao")]
    public string TipoComunicacao { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [Column("assunto")]
    public string Assunto { get; set; } = string.Empty;

    [Required]
    [Column("corpo_mensagem")]
    public string CorpoMensagem { get; set; } = string.Empty;

    [Column("data_envio")]
    public DateTime DataEnvio { get; set; } = DateTime.Now;

    [Required]
    [MaxLength(20)]
    [Column("estado")]
    public string Estado { get; set; } = "Enviada";

    // Navegação
    [ForeignKey("IdCandidatura")]
    public Candidatura Candidatura { get; set; } = null!;
}

