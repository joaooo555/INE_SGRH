using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("candidato")]
public class Candidato
{
    [Key]
    [Column("id_candidato")]
    public int IdCandidato { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("nome_completo")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(15)]
    [Column("telefone")]
    public string? Telefone { get; set; }

    [Column("data_nascimento")]
    public DateOnly? DataNascimento { get; set; }

    [MaxLength(30)]
    [Column("numero_identificacao")]
    public string? NumeroIdentificacao { get; set; }

    [Column("id_nivel_academico")]
    public int? IdNivelAcademico { get; set; }

    [MaxLength(100)]
    [Column("formacao")]
    public string? Formacao { get; set; }

    [Column("curriculum_vitae")]
    public byte[]? CurriculumVitae { get; set; }

    [Column("data_registo")]
    public DateTime DataRegisto { get; set; } = DateTime.Now;

    // Navegação
    [ForeignKey("IdNivelAcademico")]
    public NivelAcademico? NivelAcademico { get; set; }

    public ICollection<Candidatura> Candidaturas { get; set; } = [];
}

