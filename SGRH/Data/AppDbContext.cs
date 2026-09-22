using Microsoft.EntityFrameworkCore;
using SGRH.Models;

namespace SGRH.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    public DbSet<EstadoCivil> EstadosCivis { get; set; }
    public DbSet<FormaIngresso> FormasIngresso { get; set; }
    public DbSet<NivelAcademico> NiveisAcademicos { get; set; }
    public DbSet<TipoActo> TiposActo { get; set; }
    public DbSet<TipoDocumento> TiposDocumento { get; set; }
    public DbSet<TipoPedido> TiposPedido { get; set; }
    public DbSet<TipoContrato> TiposContrato { get; set; }
    public DbSet<TipoAusencia> TiposAusencia { get; set; }


    public DbSet<UnidadeOrganica> UnidadesOrganicas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Carreira> Carreiras { get; set; }
    public DbSet<Funcao> Funcoes { get; set; }


    public DbSet<Colaborador> Colaboradores { get; set; }
    public DbSet<ActoAdministrativo> ActosAdministrativos { get; set; }
    public DbSet<Documento> Documentos { get; set; }
    public DbSet<HistoricoColaborador> HistoricosColaborador { get; set; }


    public DbSet<AnuncioRecrutamento> AnunciosRecrutamento { get; set; }
    public DbSet<Candidato> Candidatos { get; set; }
    public DbSet<Candidatura> Candidaturas { get; set; }
    public DbSet<FaseProcesso> FasesProcesso { get; set; }
    public DbSet<ComunicacaoCandidato> ComunicacoesCandidato { get; set; }


    public DbSet<Contrato> Contratos { get; set; }


    public DbSet<RegistoAusencia> RegistosAusencia { get; set; }
    public DbSet<PedidoAprovacao> PedidosAprovacao { get; set; }
    public DbSet<AprovacaoPedido> AprovacoesPedido { get; set; }


    public DbSet<PlanoFormacao> PlanosFormacao { get; set; }
    public DbSet<AccaoFormacao> AccoesFormacao { get; set; }
    public DbSet<Formador> Formadores { get; set; }
    public DbSet<AccaoFormador> AccaoFormadores { get; set; }
    public DbSet<InscricaoFormacao> InscricoesFormacao { get; set; }
    public DbSet<AvaliacaoFormacao> AvaliacoesFormacao { get; set; }


    public DbSet<Estagio> Estagios { get; set; }
    public DbSet<AvaliacaoEstagio> AvaliacoesEstagio { get; set; }
    public DbSet<ActividadeEstagio> ActividadesEstagio { get; set; }


    public DbSet<ProjectoOperacao> ProjectosOperacao { get; set; }
    public DbSet<GuiaMarcha> GuiasMarcha { get; set; }


    public DbSet<CasoApoioSocial> CasosApoioSocial { get; set; }
    public DbSet<NotaAcompanhamento> NotasAcompanhamento { get; set; }


    public DbSet<InqueritoClima> InqueritosClima { get; set; }
    public DbSet<PerguntaInquerito> PerguntasInquerito { get; set; }
    public DbSet<OpcaoResposta> OpcoesResposta { get; set; }
    public DbSet<RespostaInquerito> RespostasInquerito { get; set; }


    public DbSet<PerfilAcesso> PerfisAcesso { get; set; }
    public DbSet<Permissao> Permissoes { get; set; }
    public DbSet<UtilizadorSistema> UtilizadoresSistema { get; set; }


    public DbSet<LogAuditoria> LogsAuditoria { get; set; }
    public DbSet<IndicadorCalculado> IndicadoresCalculados { get; set; }
    public DbSet<Exportacao> Exportacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<UnidadeOrganica>()
            .HasOne(u => u.UnidadePai)
            .WithMany(u => u.SubUnidades)
            .HasForeignKey(u => u.IdUnidadePai)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UnidadeOrganica>()
            .HasIndex(u => u.IdUnidadePai)
            .HasDatabaseName("idx_uo_pai");

        modelBuilder.Entity<UnidadeOrganica>()
            .Property(u => u.Estado)
            .HasDefaultValue("Ativa");


        modelBuilder.Entity<Colaborador>()
            .HasIndex(c => c.Nuit).HasDatabaseName("idx_colab_nuit");
        modelBuilder.Entity<Colaborador>()
            .HasIndex(c => c.IdUnidadeOrganica).HasDatabaseName("idx_colab_unidade");
        modelBuilder.Entity<Colaborador>()
            .HasIndex(c => c.Estado).HasDatabaseName("idx_colab_estado");

        modelBuilder.Entity<Colaborador>()
            .Property(c => c.Nacionalidade).HasDefaultValue("Moçambicana");
        modelBuilder.Entity<Colaborador>()
            .Property(c => c.Estado).HasDefaultValue("Ativo");
        modelBuilder.Entity<Colaborador>()
            .Property(c => c.DataRegisto).HasDefaultValueSql("GETDATE()");


        modelBuilder.Entity<Colaborador>()
            .HasMany(c => c.GuiasMarcha)
            .WithOne(g => g.Funcionario)
            .HasForeignKey(g => g.IdFuncionario)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Colaborador>()
            .HasMany(c => c.GuiasMarchaAutorizadas)
            .WithOne(g => g.Superior)
            .HasForeignKey(g => g.AutorizacaoSuperior)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Colaborador>()
            .HasMany(c => c.EstagiosSupervisados)
            .WithOne(e => e.Supervisor)
            .HasForeignKey(e => e.IdSupervisor)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Colaborador>()
            .HasMany(c => c.AprovacoesPedido)
            .WithOne(a => a.Aprovador)
            .HasForeignKey(a => a.IdAprovador)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<ActoAdministrativo>()
            .HasIndex(a => a.IdColaborador).HasDatabaseName("idx_acto_colab");

        modelBuilder.Entity<ActoAdministrativo>()
            .HasOne(a => a.Utilizador)
            .WithMany()
            .HasForeignKey(a => a.UtilizadorRegisto)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Documento>()
            .HasIndex(d => d.IdColaborador).HasDatabaseName("idx_doc_colab");

        modelBuilder.Entity<Documento>()
            .HasOne(d => d.Utilizador)
            .WithMany()
            .HasForeignKey(d => d.UtilizadorUpload)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<HistoricoColaborador>()
            .HasIndex(h => h.IdColaborador).HasDatabaseName("idx_hist_colab");


        modelBuilder.Entity<AnuncioRecrutamento>()
            .Property(a => a.Estado).HasDefaultValue("Aberto");
        modelBuilder.Entity<AnuncioRecrutamento>()
            .ToTable(t => t.HasCheckConstraint("CK_anuncio_datas", "[data_limite] >= [data_publicacao]"));


        modelBuilder.Entity<Candidatura>()
            .HasIndex(c => new { c.IdCandidato, c.IdAnuncio })
            .IsUnique()
            .HasDatabaseName("UQ_candidatura");

        modelBuilder.Entity<Candidatura>()
            .HasIndex(c => c.IdCandidato).HasDatabaseName("idx_cand_candidato");
        modelBuilder.Entity<Candidatura>()
            .HasIndex(c => c.IdAnuncio).HasDatabaseName("idx_cand_anuncio");
        modelBuilder.Entity<Candidatura>()
            .HasIndex(c => c.Estado).HasDatabaseName("idx_cand_estado");
        modelBuilder.Entity<Candidatura>()
            .Property(c => c.Estado).HasDefaultValue("Submetida");


        modelBuilder.Entity<FaseProcesso>()
            .HasIndex(f => f.IdCandidatura).HasDatabaseName("idx_fase_candidatura");


        modelBuilder.Entity<ComunicacaoCandidato>()
            .HasIndex(c => c.IdCandidatura).HasDatabaseName("idx_comunic_candidatura");


        modelBuilder.Entity<Contrato>()
            .HasIndex(c => c.IdColaborador).HasDatabaseName("idx_ctr_colab");
        modelBuilder.Entity<Contrato>()
            .HasIndex(c => c.Estado).HasDatabaseName("idx_ctr_estado");
        modelBuilder.Entity<Contrato>()
            .HasIndex(c => c.DataFim).HasDatabaseName("idx_ctr_datafim");
        modelBuilder.Entity<Contrato>()
            .Property(c => c.Estado).HasDefaultValue("Vigente");
        modelBuilder.Entity<Contrato>()
            .ToTable(t => t.HasCheckConstraint("CK_contrato_datas", "[data_fim] IS NULL OR [data_fim] >= [data_inicio]"));

        modelBuilder.Entity<Contrato>()
            .HasOne(c => c.ContratoAnterior)
            .WithMany(c => c.Renovacoes)
            .HasForeignKey(c => c.RenovacaoAnterior)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<RegistoAusencia>()
            .HasIndex(r => r.IdColaborador).HasDatabaseName("idx_aus_colab");
        modelBuilder.Entity<RegistoAusencia>()
            .HasIndex(r => r.Estado).HasDatabaseName("idx_aus_estado");
        modelBuilder.Entity<RegistoAusencia>()
            .ToTable(t => t.HasCheckConstraint("CK_ausencia_datas", "[data_fim] >= [data_inicio]"));


        modelBuilder.Entity<PedidoAprovacao>()
            .HasIndex(p => p.IdColaboradorSolicitante).HasDatabaseName("idx_ped_solicitante");


        modelBuilder.Entity<AprovacaoPedido>()
            .HasIndex(a => a.IdPedido).HasDatabaseName("idx_aprov_pedido");


        modelBuilder.Entity<AccaoFormacao>()
            .HasIndex(a => a.IdPlano).HasDatabaseName("idx_accao_plano");


        modelBuilder.Entity<AccaoFormador>()
            .HasKey(af => new { af.IdAccao, af.IdFormador });


        modelBuilder.Entity<InscricaoFormacao>()
            .HasIndex(i => new { i.IdAccao, i.IdColaborador })
            .IsUnique()
            .HasDatabaseName("UQ_inscricao");

        modelBuilder.Entity<InscricaoFormacao>()
            .HasIndex(i => i.IdAccao).HasDatabaseName("idx_insc_accao");
        modelBuilder.Entity<InscricaoFormacao>()
            .HasIndex(i => i.IdColaborador).HasDatabaseName("idx_insc_colab");


        modelBuilder.Entity<AvaliacaoFormacao>()
            .HasOne(a => a.InscricaoFormacao)
            .WithOne(i => i.AvaliacaoFormacao)
            .HasForeignKey<AvaliacaoFormacao>(a => a.IdInscricao)
            .OnDelete(DeleteBehavior.Restrict);





        modelBuilder.Entity<Estagio>()
            .HasIndex(e => e.IdUnidadeOrganica).HasDatabaseName("idx_estag_unidade");
        modelBuilder.Entity<Estagio>()
            .HasIndex(e => e.IdSupervisor).HasDatabaseName("idx_estag_supervisor");


        modelBuilder.Entity<AvaliacaoEstagio>()
            .HasIndex(a => a.IdEstagio).HasDatabaseName("idx_avales_estag");


        modelBuilder.Entity<ActividadeEstagio>()
            .HasIndex(a => a.IdEstagio).HasDatabaseName("idx_actest_estag");


        modelBuilder.Entity<GuiaMarcha>()
            .HasIndex(g => g.IdFuncionario).HasDatabaseName("idx_gm_func");
        modelBuilder.Entity<GuiaMarcha>()
            .HasIndex(g => g.Estado).HasDatabaseName("idx_gm_estado");
        modelBuilder.Entity<GuiaMarcha>()
            .HasIndex(g => new { g.DataPartida, g.DataChegada }).HasDatabaseName("idx_gm_datas");


        modelBuilder.Entity<CasoApoioSocial>()
            .HasIndex(c => c.IdColaborador).HasDatabaseName("idx_caso_colab");
        modelBuilder.Entity<CasoApoioSocial>()
            .HasIndex(c => c.Estado).HasDatabaseName("idx_caso_estado");


        modelBuilder.Entity<NotaAcompanhamento>()
            .HasIndex(n => n.IdCaso).HasDatabaseName("idx_nota_caso");


        modelBuilder.Entity<InqueritoClima>()
            .Property(i => i.Estado).HasDefaultValue("Rascunho");


        modelBuilder.Entity<PerguntaInquerito>()
            .HasIndex(p => p.IdInquerito).HasDatabaseName("idx_perg_inq");


        modelBuilder.Entity<OpcaoResposta>()
            .HasIndex(o => o.IdPergunta).HasDatabaseName("idx_opcao_pergunta");


        modelBuilder.Entity<RespostaInquerito>()
            .HasIndex(r => r.IdPergunta).HasDatabaseName("idx_resp_perg");


        modelBuilder.Entity<Permissao>()
            .HasIndex(p => new { p.IdPerfil, p.Modulo })
            .IsUnique()
            .HasDatabaseName("UQ_permissao");

        modelBuilder.Entity<Permissao>()
            .HasIndex(p => p.IdPerfil).HasDatabaseName("idx_perm_perfil");


        modelBuilder.Entity<UtilizadorSistema>()
            .HasIndex(u => u.Username).IsUnique().HasDatabaseName("idx_util_username");
        modelBuilder.Entity<UtilizadorSistema>()
            .HasIndex(u => u.IdPerfil).HasDatabaseName("idx_util_perfil");
        modelBuilder.Entity<UtilizadorSistema>()
            .Property(u => u.Estado).HasDefaultValue("Ativo");

        modelBuilder.Entity<UtilizadorSistema>()
            .HasOne(u => u.Colaborador)
            .WithOne(c => c.UtilizadorSistema)
            .HasForeignKey<UtilizadorSistema>(u => u.IdColaborador)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<LogAuditoria>()
            .HasIndex(l => l.DataHora).HasDatabaseName("idx_audit_data");
        modelBuilder.Entity<LogAuditoria>()
            .HasIndex(l => l.TabelaAfetada).HasDatabaseName("idx_audit_tabela");

        modelBuilder.Entity<LogAuditoria>()
            .HasOne(l => l.Colaborador)
            .WithMany(c => c.LogsAuditoria)
            .HasForeignKey(l => l.IdColaborador)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LogAuditoria>()
            .HasOne(l => l.UtilizadorSistema)
            .WithMany(u => u.LogsAuditoria)
            .HasForeignKey(l => l.IdUtilizador)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Exportacao>()
            .HasIndex(e => e.IdUtilizador).HasDatabaseName("idx_export_util");




        modelBuilder.Entity<EstadoCivil>().HasData(
            new EstadoCivil { IdEstadoCivil = 1, Nome = "Solteiro(a)" },
            new EstadoCivil { IdEstadoCivil = 2, Nome = "Casado(a)" },
            new EstadoCivil { IdEstadoCivil = 3, Nome = "Divorciado(a)" },
            new EstadoCivil { IdEstadoCivil = 4, Nome = "Viúvo(a)" },
            new EstadoCivil { IdEstadoCivil = 5, Nome = "União de Facto" }
        );


        modelBuilder.Entity<FormaIngresso>().HasData(
            new FormaIngresso { IdFormaIngresso = 1, Nome = "Concurso Público" },
            new FormaIngresso { IdFormaIngresso = 2, Nome = "Nomeação" },
            new FormaIngresso { IdFormaIngresso = 3, Nome = "Contratação a Termo Certo" },
            new FormaIngresso { IdFormaIngresso = 4, Nome = "Contratação a Termo Indeterminado" },
            new FormaIngresso { IdFormaIngresso = 5, Nome = "Destacamento" },
            new FormaIngresso { IdFormaIngresso = 6, Nome = "Mobilidade" }
        );


        modelBuilder.Entity<TipoContrato>().HasData(
            new TipoContrato { IdTipoContrato = 1, Nome = "Contrato por Tempo Indeterminado", Descricao = "Contrato sem termo final", DuracaoMaximaMeses = null },
            new TipoContrato { IdTipoContrato = 2, Nome = "Contrato a Termo Certo", Descricao = "Contrato com data de fim definida", DuracaoMaximaMeses = 24 },
            new TipoContrato { IdTipoContrato = 3, Nome = "Contrato a Termo Incerto", Descricao = "Contrato para substituição temporária", DuracaoMaximaMeses = 12 },
            new TipoContrato { IdTipoContrato = 4, Nome = "Contrato de Prestação de Serviços", Descricao = "Prestação de serviços especializados", DuracaoMaximaMeses = 12 },
            new TipoContrato { IdTipoContrato = 5, Nome = "Nomeação", Descricao = "Nomeação em funções públicas", DuracaoMaximaMeses = null },
            new TipoContrato { IdTipoContrato = 6, Nome = "Comissão de Serviço", Descricao = "Comissão temporária de serviço", DuracaoMaximaMeses = 12 }
        );


        modelBuilder.Entity<UnidadeOrganica>().HasData(
            new UnidadeOrganica { IdUnidadeOrganica = 1, Nome = "Presidência do INE", Sigla = "PRES", Tipo = "Departamento", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 2, Nome = "Departamento de Apoio ao Director Geral", Sigla = "DADG", Tipo = "Departamento", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 3, Nome = "Departamento de Recursos Humanos, Administração e Finanças", Sigla = "DRHAF", Tipo = "Departamento", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 4, Nome = "Departamento de Tecnologias de Informação", Sigla = "DTI", Tipo = "Departamento", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 5, Nome = "Departamento de Contas Nacionais e Coordenação Estatística", Sigla = "DCNCE", Tipo = "Departamento", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 6, Nome = "Departamento de Estatísticas Económicas e Financeiras", Sigla = "DEEF", Tipo = "Departamento", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 7, Nome = "Departamento de Estatísticas Demográficas e Sociais", Sigla = "DEDS", Tipo = "Departamento", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 8, Nome = "Departamento de Censos e Inquéritos Especiais", Sigla = "DCIE", Tipo = "Departamento", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 9, Nome = "Departamento de Informação e Difusão", Sigla = "DID", Tipo = "Departamento", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 10, Nome = "Escola Nacional de Estatística", Sigla = "ENE", Tipo = "Escola", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 11, Nome = "Delegação Provincial - Niassa", Sigla = "DP-NIA", Tipo = "Delegação", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 12, Nome = "Delegação Provincial - Cabo Delgado", Sigla = "DP-CDL", Tipo = "Delegação", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 13, Nome = "Delegação Provincial - Nampula", Sigla = "DP-NAM", Tipo = "Delegação", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 14, Nome = "Delegação Provincial - Zambézia", Sigla = "DP-ZAM", Tipo = "Delegação", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 15, Nome = "Delegação Provincial - Tete", Sigla = "DP-TET", Tipo = "Delegação", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 16, Nome = "Delegação Provincial - Manica", Sigla = "DP-MAN", Tipo = "Delegação", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 17, Nome = "Delegação Provincial - Sofala", Sigla = "DP-SOF", Tipo = "Delegação", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 18, Nome = "Delegação Provincial - Inhambane", Sigla = "DP-INH", Tipo = "Delegação", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 19, Nome = "Delegação Provincial - Gaza", Sigla = "DP-GAZ", Tipo = "Delegação", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 20, Nome = "Delegação Provincial - Maputo Província", Sigla = "DP-MP", Tipo = "Delegação", Estado = "Ativa" },
            new UnidadeOrganica { IdUnidadeOrganica = 21, Nome = "Delegação Provincial - Maputo Cidade", Sigla = "DP-MC", Tipo = "Delegação", Estado = "Ativa" }
        );


        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { IdCategoria = 1, Descricao = "Assessor Principal", Nivel = 1 },
            new Categoria { IdCategoria = 2, Descricao = "Primeiro Assessor", Nivel = 2 },
            new Categoria { IdCategoria = 3, Descricao = "Assessor", Nivel = 3 },
            new Categoria { IdCategoria = 4, Descricao = "Técnico Superior Principal", Nivel = 4 },
            new Categoria { IdCategoria = 5, Descricao = "Técnico Superior de 1ª Classe", Nivel = 5 },
            new Categoria { IdCategoria = 6, Descricao = "Técnico Superior de 2ª Classe", Nivel = 6 },
            new Categoria { IdCategoria = 7, Descricao = "Especialista Principal", Nivel = 7 },
            new Categoria { IdCategoria = 8, Descricao = "Especialista de 1ª Classe", Nivel = 8 },
            new Categoria { IdCategoria = 9, Descricao = "Especialista de 2ª Classe", Nivel = 9 },
            new Categoria { IdCategoria = 10, Descricao = "Técnico de 1ª Classe", Nivel = 10 },
            new Categoria { IdCategoria = 11, Descricao = "Técnico de 2ª Classe", Nivel = 11 },
            new Categoria { IdCategoria = 12, Descricao = "Técnico de 3ª Classe", Nivel = 12 },
            new Categoria { IdCategoria = 13, Descricao = "Técnico Médio Principal", Nivel = 13 },
            new Categoria { IdCategoria = 14, Descricao = "Técnico Médio de 1ª Classe", Nivel = 14 },
            new Categoria { IdCategoria = 15, Descricao = "Técnico Médio de 2ª Classe", Nivel = 15 },
            new Categoria { IdCategoria = 16, Descricao = "Técnico Médio de 3ª Classe", Nivel = 16 },
            new Categoria { IdCategoria = 17, Descricao = "Oficial Administrativo Principal", Nivel = 17 },
            new Categoria { IdCategoria = 18, Descricao = "1º Oficial Administrativo", Nivel = 18 },
            new Categoria { IdCategoria = 19, Descricao = "2º Oficial Administrativo", Nivel = 19 },
            new Categoria { IdCategoria = 20, Descricao = "3º Oficial Administrativo", Nivel = 20 },
            new Categoria { IdCategoria = 21, Descricao = "Auxiliar Administrativo", Nivel = 21 },
            new Categoria { IdCategoria = 22, Descricao = "Agente de Serviço", Nivel = 22 }
        );


        modelBuilder.Entity<Carreira>().HasData(
            new Carreira { IdCarreira = 1, Nome = "Carreira de Direcção e Confiança", Descricao = "Funções de direcção e confiança política" },
            new Carreira { IdCarreira = 2, Nome = "Carreira de Estatística", Descricao = "Funções técnicas de estatística" },
            new Carreira { IdCarreira = 3, Nome = "Carreira de Informática", Descricao = "Funções técnicas de informática" },
            new Carreira { IdCarreira = 4, Nome = "Carreira Administrativa", Descricao = "Funções administrativas e de gestão" },
            new Carreira { IdCarreira = 5, Nome = "Carreira de Investigação Científica", Descricao = "Funções de investigação e pesquisa" },
            new Carreira { IdCarreira = 6, Nome = "Regime Especial Não Diferenciado", Descricao = "Regime especial sem diferenciação de carreira" }
        );


        modelBuilder.Entity<Funcao>().HasData(
            new Funcao { IdFuncao = 1, Nome = "Director Geral", Descricao = "Responsável máximo pela gestão do INE" },
            new Funcao { IdFuncao = 2, Nome = "Director Geral Adjunto", Descricao = "Adjunto do Director Geral" },
            new Funcao { IdFuncao = 3, Nome = "Chefe de Departamento", Descricao = "Responsável por um departamento" },
            new Funcao { IdFuncao = 4, Nome = "Chefe de Secção", Descricao = "Responsável por uma secção" },
            new Funcao { IdFuncao = 5, Nome = "Estatístico", Descricao = "Profissional de estatística" },
            new Funcao { IdFuncao = 6, Nome = "Técnico de Estatística", Descricao = "Técnico em operações estatísticas" },
            new Funcao { IdFuncao = 7, Nome = "Analista de Dados", Descricao = "Análise e tratamento de dados" },
            new Funcao { IdFuncao = 8, Nome = "Cartógrafo", Descricao = "Elaboração de mapas e cartografia" },
            new Funcao { IdFuncao = 9, Nome = "Programador", Descricao = "Desenvolvimento de software" },
            new Funcao { IdFuncao = 10, Nome = "Técnico de Informática", Descricao = "Suporte e manutenção de TI" },
            new Funcao { IdFuncao = 11, Nome = "Administrador de Sistemas", Descricao = "Gestão de sistemas informáticos" },
            new Funcao { IdFuncao = 12, Nome = "Contabilista", Descricao = "Gestão contabilística e financeira" },
            new Funcao { IdFuncao = 13, Nome = "Técnico de Administração", Descricao = "Funções administrativas" },
            new Funcao { IdFuncao = 14, Nome = "Assistente Administrativo", Descricao = "Apoio administrativo" },
            new Funcao { IdFuncao = 15, Nome = "Secretário de Direcção", Descricao = "Apoio à direcção" },
            new Funcao { IdFuncao = 16, Nome = "Motorista", Descricao = "Serviço de transporte" },
            new Funcao { IdFuncao = 17, Nome = "Agente de Segurança", Descricao = "Vigilância e segurança" },
            new Funcao { IdFuncao = 18, Nome = "Auxiliar de Limpeza", Descricao = "Serviços de limpeza e conservação" }
        );

    }
}

