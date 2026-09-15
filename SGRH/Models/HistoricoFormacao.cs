using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGRH.Models;

/// <summary>
/// ELIMINADO por violação 3FN — tabela inteiramente derivada de:
///   inscricao_formacao JOIN avaliacao_formacao JOIN accao_formacao
///
/// Utilizar a View SQL (criar via migration manual):
///   CREATE VIEW vw_historico_formacao AS
///   SELECT i.id_colaborador, i.id_accao, a.data_fim AS data_conclusao,
///          av.nota, av.certificado
///   FROM inscricao_formacao i
///   JOIN accao_formacao a ON i.id_accao = a.id_accao
///   LEFT JOIN avaliacao_formacao av ON av.id_inscricao = i.id_inscricao
///   WHERE i.estado = 'Concluído';
/// </summary>
[Obsolete("Tabela eliminada — violação 3FN. Ver comentário acima para a View equivalente.")]
public class HistoricoFormacao
{
    // Classe mantida apenas para referência histórica.
    // NÃO registar no DbContext.
}
