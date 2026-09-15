using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

[Table("nivel_academico")]
public class NivelAcademico
{
    [Key]
    [Column("id_nivel_academico")]
    public int IdNivelAcademico { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    // Navegação
    public ICollection<Candidato> Candidatos { get; set; } = [];
}

