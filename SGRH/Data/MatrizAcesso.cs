using Microsoft.EntityFrameworkCore;
using SGRH.Models;

namespace SGRH.Data;

/// <summary>
/// Níveis de acesso por perfil e módulo, conforme a matriz funcional do SGRH.
///
/// A matriz é a fonte de verdade: os quatro booleanos da tabela <c>permissao</c>
/// (pode_visualizar, pode_criar, pode_editar, pode_eliminar) são DERIVADOS do
/// nível, para que a autorização já existente (AutorizacaoService,
/// [VerificarPermissao], menu lateral) continue a funcionar sem alterações.
/// </summary>
public static class MatrizAcesso
{
    // ── Módulos do sistema ───────────────────────────────────────────────
    // Os nomes são os que o código já usa em PodeVer(...) e em
    // [VerificarPermissao(Modulo = "...")]. A matriz funcional tem 10 áreas;
    // "Cadastro" corresponde a Colaboradores e "Administração do Sistema"
    // cobre Utilizadores + Auditoria + Configuracoes.
    public const string Dashboard = "Dashboard";
    public const string Cadastro = "Colaboradores";          // Cadastro e Processo Individual
    public const string Recrutamento = "Recrutamento";        // Recrutamento e Selecção
    public const string Contratos = "Contratos";              // Gestão de Contratos
    public const string Administracao = "Administracao";      // Administração de Pessoal
    public const string Formacao = "Formacao";                // Formação e Desenvolvimento
    public const string Estagios = "Estagios";                // Gestão de Estágios
    public const string GuiasMarcha = "GuiasMarcha";          // (fora da matriz funcional)
    public const string Clima = "Clima";                      // Clima Organizacional
    public const string AssuntosSociais = "AssuntosSociais";  // Assuntos Sociais e Bem-estar
    public const string Reporting = "Reporting";              // Reporting e Analytics
    public const string Utilizadores = "Utilizadores";        // Administração do Sistema
    public const string Auditoria = "Auditoria";              // Administração do Sistema
    public const string Configuracoes = "Configuracoes";      // Administração do Sistema

    public static readonly IReadOnlyList<string> Modulos = new[]
    {
        Dashboard, Cadastro, Recrutamento, Contratos, Administracao, Formacao,
        Estagios, GuiasMarcha, Clima, AssuntosSociais, Reporting,
        Utilizadores, Auditoria, Configuracoes
    };

    // ── Níveis de acesso ─────────────────────────────────────────────────
    public const string Nao = "Não";
    public const string Consulta = "Consulta";
    public const string ConsultaNecessaria = "Consulta necessária";
    public const string ConsultaAgregada = "Consulta agregada";
    public const string Proprio = "Próprio";
    public const string PropriosDados = "Próprios dados";
    public const string Limitado = "Limitado";
    public const string SeSupervisor = "Se supervisor";
    public const string Restrito = "Restrito";
    public const string Aprovacao = "Aprovação";
    public const string Solicitacao = "Solicitação";
    public const string Participacao = "Participação";
    public const string Avaliacao = "Avaliação";
    public const string Elevado = "Elevado";
    public const string Total = "Total";
    public const string Tecnico = "Técnico";
    public const string TecnicoRestrito = "Técnico restrito";

    /// <summary>
    /// Converte um nível de acesso nos quatro booleanos de operação.
    /// Níveis desconhecais caem em "Consulta" (conservador: só visualizar).
    /// </summary>
    public static (bool Visualizar, bool Criar, bool Editar, bool Eliminar) ParaOperacoes(string? nivel) => nivel switch
    {
        Nao => (false, false, false, false),
        Total => (true, true, true, true),
        Tecnico => (true, true, true, true),
        Elevado => (true, true, true, false),
        SeSupervisor => (true, true, true, false),
        Aprovacao => (true, false, true, false),
        Solicitacao => (true, true, false, false),
        Participacao => (true, true, true, false),
        Avaliacao => (true, true, true, false),
        Consulta => (true, false, false, false),
        ConsultaNecessaria => (true, false, false, false),
        ConsultaAgregada => (true, false, false, false),
        Proprio => (true, false, false, false),
        PropriosDados => (true, false, false, false),
        Limitado => (true, false, false, false),
        Restrito => (true, false, false, false),
        TecnicoRestrito => (true, false, false, false),
        // Níveis de âmbito ("Reporting de Contratos", "Reporting de Estágios", ...)
        // guardam o texto original na matriz mas concedem apenas leitura.
        _ => (true, false, false, false)
    };

    /// <summary>Descrição de cada nível, para tooltips na interface.</summary>
    public static string Descricao(string nivel) => nivel switch
    {
        Total => "Criar, consultar, actualizar, eliminar quando permitido e gerir o processo",
        Tecnico => "Acesso técnico total a todos os módulos",
        Elevado => "Criar/actualizar/consultar dentro da área de responsabilidade",
        SeSupervisor => "Apenas sobre os colaboradores que supervisiona",
        Aprovacao => "Consultar e aprovar/recusar pedidos da equipa",
        Solicitacao => "Consultar e submeter pedidos (férias, ausências, etc.)",
        Participacao => "Participar nos inquéritos de clima organizacional",
        Avaliacao => "Avaliar candidatos no processo de recrutamento",
        Consulta => "Apenas visualizar informações autorizadas",
        ConsultaNecessaria => "Visualizar apenas quando necessário ao processo",
        ConsultaAgregada => "Consultar apenas dados agregados (sem dados pessoais)",
        Proprio => "Apenas informações do próprio utilizador",
        PropriosDados => "Apenas dados próprios em reporting",
        Limitado => "Funcionalidades específicas do processo",
        Restrito => "Acesso restrito, sem escrita",
        TecnicoRestrito => "Acesso técnico restrito, sem escrita",
        _ => "Sem acesso ao módulo"
    };

    // ── A matriz ─────────────────────────────────────────────────────────
    // Indexada por id_perfil (e não por nome) porque é o seed que fixa a
    // numeração 1..13. Bases de dados criadas por versões anteriores do
    // protótipo têm nomes divergentes para os mesmos ids.
    //
    // Ordem das colunas da matriz funcional:
    //   Cadastro, Recrutamento, Contratos, Administração, Formação, Estágios,
    //   Clima, Assuntos Sociais, Reporting, Administração do Sistema
    private static readonly Dictionary<int, (string Nome, string[] Colunas)> _perfis = new()
    {
        [1] = ("Administrador", new[]
        {
            Total, Total, Total, Total, Total, Total, Total, Total, Total, Total
        }),
        [2] = ("RH", new[]
        {
            Total, Total, Total, Total, Total, Total, Total, Restrito, Elevado, Nao
        }),
        [3] = ("Gestor", new[]
        {
            Consulta, Consulta, Consulta, Aprovacao, Consulta, SeSupervisor, Consulta, Nao, Consulta, Nao
        }),
        [4] = ("Direcção", new[]
        {
            Consulta, Consulta, Consulta, Consulta, Consulta, Consulta, Consulta, ConsultaAgregada, Elevado, Nao
        }),
        [5] = ("Funcionário/FAE", new[]
        {
            Proprio, Consulta, Proprio, Solicitacao, Proprio, Proprio, Participacao, Proprio, PropriosDados, Nao
        }),
        [6] = ("Júri", new[]
        {
            Nao, Avaliacao, Nao, Nao, Nao, Nao, Nao, Nao, "Recrutamento", Nao
        }),
        [7] = ("Jurídico", new[]
        {
            ConsultaNecessaria, Nao, Elevado, Nao, Nao, Nao, Nao, Nao, "Contratos", Nao
        }),
        [8] = ("Formador", new[]
        {
            ConsultaNecessaria, Nao, Nao, Nao, Elevado, Nao, Nao, Nao, "Formação", Nao
        }),
        [9] = ("Supervisor", new[]
        {
            ConsultaNecessaria, Nao, Nao, Nao, Nao, Elevado, Nao, Nao, "Estágios", Nao
        }),
        [10] = ("Estagiário", new[]
        {
            Proprio, Nao, Nao, Nao, Nao, Limitado, Nao, Nao, PropriosDados, Nao
        }),
        [11] = ("Candidato", new[]
        {
            Nao, Proprio, Nao, Nao, Nao, Nao, Nao, Nao, Nao, Nao
        }),
        [12] = ("Instituição de Ensino", new[]
        {
            Nao, Nao, Nao, Nao, Nao, Limitado, Nao, Nao, "Estágios", Nao
        }),
        [13] = ("TI", new[]
        {
            Tecnico, Tecnico, Tecnico, Tecnico, Tecnico, Tecnico, Tecnico, TecnicoRestrito, Tecnico, Tecnico
        })
    };

    /// <summary>Nomes dos perfis da matriz, na ordem da tabela funcional.</summary>
    public static IReadOnlyList<string> NomesPerfis => _perfis.Values.Select(p => p.Nome).ToList();

    /// <summary>
    /// Deriva as permissões de um perfil a partir da matriz.
    /// Dashboard não consta da matriz funcional: concede-se "Consulta" a todos,
    /// porque o painel é a página de entrada e sem ela o login não tem destino.
    /// GuiasMarcha também não consta: herda o nível de Cadastro do próprio perfil.
    /// </summary>
    public static List<Permissao> PermissoesDoPerfil(int idPerfil)
    {
        if (!_perfis.TryGetValue(idPerfil, out var perfil))
            return [];

        var c = perfil.Colunas;
        var nivelCadastro = c[0];
        var adminSistema = c[9];

        var nivelPorModulo = new Dictionary<string, string>
        {
            [Dashboard] = Consulta,
            [Cadastro] = c[0],
            [Recrutamento] = c[1],
            [Contratos] = c[2],
            [Administracao] = c[3],
            [Formacao] = c[4],
            [Estagios] = c[5],
            [GuiasMarcha] = nivelCadastro,
            [Clima] = c[6],
            [AssuntosSociais] = c[7],
            [Reporting] = c[8],
            [Utilizadores] = adminSistema,
            [Auditoria] = adminSistema,
            [Configuracoes] = adminSistema
        };

        return Modulos.Select(modulo =>
        {
            var nivel = nivelPorModulo[modulo];
            var (visualizar, criar, editar, eliminar) = ParaOperacoes(nivel);
            return new Permissao
            {
                IdPerfil = idPerfil,
                Modulo = modulo,
                NivelAcesso = nivel,
                PodeVisualizar = visualizar,
                PodeCriar = criar,
                PodeEditar = editar,
                PodeEliminar = eliminar
            };
        }).ToList();
    }

    /// <summary>
    /// Reescreve a tabela permissao com a matriz. É deliberadamente
    /// destrutivo: a matriz é a fonte de verdade, tal como o seed já fazia
    /// com a palavra-passe do administrador.
    /// </summary>
    public static async Task AplicarAsync(AppDbContext db)
    {
        var perfis = await db.PerfisAcesso.ToListAsync();
        if (perfis.Count == 0)
            return;

        var ids = _perfis.Keys
            .Where(id => perfis.Any(p => p.IdPerfil == id))
            .ToList();

        if (ids.Count == 0)
            return;

        await db.Permissoes
            .Where(p => ids.Contains(p.IdPerfil))
            .ExecuteDeleteAsync();

        var linhas = ids
            .SelectMany(PermissoesDoPerfil)
            .ToList();

        await db.Permissoes.AddRangeAsync(linhas);
        await db.SaveChangesAsync();
    }
}
