using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGRH.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "carreira",
                columns: table => new
                {
                    id_carreira = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    classe_inicial = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    classe_final = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carreira", x => x.id_carreira);
                });

            migrationBuilder.CreateTable(
                name: "categoria",
                columns: table => new
                {
                    id_categoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    descricao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    nivel = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoria", x => x.id_categoria);
                });

            migrationBuilder.CreateTable(
                name: "estado_civil",
                columns: table => new
                {
                    id_estado_civil = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estado_civil", x => x.id_estado_civil);
                });

            migrationBuilder.CreateTable(
                name: "forma_ingresso",
                columns: table => new
                {
                    id_forma_ingresso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forma_ingresso", x => x.id_forma_ingresso);
                });

            migrationBuilder.CreateTable(
                name: "formador",
                columns: table => new
                {
                    id_formador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    especialidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    instituicao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_formador", x => x.id_formador);
                });

            migrationBuilder.CreateTable(
                name: "funcao",
                columns: table => new
                {
                    id_funcao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    remuneracao_base = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funcao", x => x.id_funcao);
                });

            migrationBuilder.CreateTable(
                name: "indicador_calculado",
                columns: table => new
                {
                    id_indicador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    formula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    periodo_referencia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    valor_calculado = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    data_calculo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_indicador_calculado", x => x.id_indicador);
                });

            migrationBuilder.CreateTable(
                name: "inquerito_clima",
                columns: table => new
                {
                    id_inquerito = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Rascunho"),
                    anonimato_garantido = table.Column<bool>(type: "bit", nullable: false),
                    utilizador_criador = table.Column<int>(type: "int", nullable: true),
                    data_registo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inquerito_clima", x => x.id_inquerito);
                });

            migrationBuilder.CreateTable(
                name: "nivel_academico",
                columns: table => new
                {
                    id_nivel_academico = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nivel_academico", x => x.id_nivel_academico);
                });

            migrationBuilder.CreateTable(
                name: "perfil_acesso",
                columns: table => new
                {
                    id_perfil = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    nivel_confidencialidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfil_acesso", x => x.id_perfil);
                });

            migrationBuilder.CreateTable(
                name: "plano_formacao",
                columns: table => new
                {
                    id_plano = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    designacao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ano = table.Column<int>(type: "int", nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: true),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    utilizador_criador = table.Column<int>(type: "int", nullable: true),
                    data_registo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plano_formacao", x => x.id_plano);
                });

            migrationBuilder.CreateTable(
                name: "projecto_operacao",
                columns: table => new
                {
                    id_projecto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: true),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projecto_operacao", x => x.id_projecto);
                });

            migrationBuilder.CreateTable(
                name: "tipo_acto",
                columns: table => new
                {
                    id_tipo_acto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_acto", x => x.id_tipo_acto);
                });

            migrationBuilder.CreateTable(
                name: "tipo_ausencia",
                columns: table => new
                {
                    id_tipo_ausencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    dias_maximos = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_ausencia", x => x.id_tipo_ausencia);
                });

            migrationBuilder.CreateTable(
                name: "tipo_contrato",
                columns: table => new
                {
                    id_tipo_contrato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    duracao_maxima_meses = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_contrato", x => x.id_tipo_contrato);
                });

            migrationBuilder.CreateTable(
                name: "tipo_documento",
                columns: table => new
                {
                    id_tipo_documento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_documento", x => x.id_tipo_documento);
                });

            migrationBuilder.CreateTable(
                name: "tipo_pedido",
                columns: table => new
                {
                    id_tipo_pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_pedido", x => x.id_tipo_pedido);
                });

            migrationBuilder.CreateTable(
                name: "unidade_organica",
                columns: table => new
                {
                    id_unidade_organica = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    sigla = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    id_unidade_pai = table.Column<int>(type: "int", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Ativa")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unidade_organica", x => x.id_unidade_organica);
                    table.ForeignKey(
                        name: "FK_unidade_organica_unidade_organica_id_unidade_pai",
                        column: x => x.id_unidade_pai,
                        principalTable: "unidade_organica",
                        principalColumn: "id_unidade_organica",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pergunta_inquerito",
                columns: table => new
                {
                    id_pergunta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_inquerito = table.Column<int>(type: "int", nullable: false),
                    texto_pergunta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tipo_pergunta = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ordem = table.Column<int>(type: "int", nullable: false),
                    obrigatoria = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pergunta_inquerito", x => x.id_pergunta);
                    table.ForeignKey(
                        name: "FK_pergunta_inquerito_inquerito_clima_id_inquerito",
                        column: x => x.id_inquerito,
                        principalTable: "inquerito_clima",
                        principalColumn: "id_inquerito",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "candidato",
                columns: table => new
                {
                    id_candidato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome_completo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    data_nascimento = table.Column<DateOnly>(type: "date", nullable: true),
                    numero_identificacao = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    id_nivel_academico = table.Column<int>(type: "int", nullable: true),
                    formacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    curriculum_vitae = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    data_registo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candidato", x => x.id_candidato);
                    table.ForeignKey(
                        name: "FK_candidato_nivel_academico_id_nivel_academico",
                        column: x => x.id_nivel_academico,
                        principalTable: "nivel_academico",
                        principalColumn: "id_nivel_academico");
                });

            migrationBuilder.CreateTable(
                name: "permissao",
                columns: table => new
                {
                    id_permissao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_perfil = table.Column<int>(type: "int", nullable: false),
                    modulo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    pode_visualizar = table.Column<bool>(type: "bit", nullable: false),
                    pode_criar = table.Column<bool>(type: "bit", nullable: false),
                    pode_editar = table.Column<bool>(type: "bit", nullable: false),
                    pode_eliminar = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissao", x => x.id_permissao);
                    table.ForeignKey(
                        name: "FK_permissao_perfil_acesso_id_perfil",
                        column: x => x.id_perfil,
                        principalTable: "perfil_acesso",
                        principalColumn: "id_perfil",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accao_formacao",
                columns: table => new
                {
                    id_accao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_plano = table.Column<int>(type: "int", nullable: true),
                    nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tema = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    local = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    vagas_disponiveis = table.Column<int>(type: "int", nullable: true),
                    horas = table.Column<decimal>(type: "decimal(5,1)", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accao_formacao", x => x.id_accao);
                    table.ForeignKey(
                        name: "FK_accao_formacao_plano_formacao_id_plano",
                        column: x => x.id_plano,
                        principalTable: "plano_formacao",
                        principalColumn: "id_plano");
                });

            migrationBuilder.CreateTable(
                name: "anuncio_recrutamento",
                columns: table => new
                {
                    id_anuncio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requisitos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quantidade_vagas = table.Column<int>(type: "int", nullable: false),
                    data_publicacao = table.Column<DateOnly>(type: "date", nullable: false),
                    data_limite = table.Column<DateOnly>(type: "date", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Aberto"),
                    id_unidade_organica = table.Column<int>(type: "int", nullable: true),
                    utilizador_criador = table.Column<int>(type: "int", nullable: true),
                    data_registo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anuncio_recrutamento", x => x.id_anuncio);
                    table.CheckConstraint("CK_anuncio_datas", "[data_limite] >= [data_publicacao]");
                    table.ForeignKey(
                        name: "FK_anuncio_recrutamento_unidade_organica_id_unidade_organica",
                        column: x => x.id_unidade_organica,
                        principalTable: "unidade_organica",
                        principalColumn: "id_unidade_organica");
                });

            migrationBuilder.CreateTable(
                name: "colaborador",
                columns: table => new
                {
                    id_colaborador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome_completo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    nuit = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    data_nascimento = table.Column<DateOnly>(type: "date", nullable: false),
                    sexo = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    id_estado_civil = table.Column<int>(type: "int", nullable: true),
                    nacionalidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Moçambicana"),
                    numero_identificacao = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    contacto_telefonico = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    contacto_email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    endereco_residencia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Ativo"),
                    id_unidade_organica = table.Column<int>(type: "int", nullable: true),
                    id_categoria = table.Column<int>(type: "int", nullable: true),
                    id_carreira = table.Column<int>(type: "int", nullable: true),
                    id_funcao = table.Column<int>(type: "int", nullable: true),
                    data_ingresso = table.Column<DateOnly>(type: "date", nullable: false),
                    id_forma_ingresso = table.Column<int>(type: "int", nullable: true),
                    data_registo = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_colaborador", x => x.id_colaborador);
                    table.ForeignKey(
                        name: "FK_colaborador_carreira_id_carreira",
                        column: x => x.id_carreira,
                        principalTable: "carreira",
                        principalColumn: "id_carreira");
                    table.ForeignKey(
                        name: "FK_colaborador_categoria_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "categoria",
                        principalColumn: "id_categoria");
                    table.ForeignKey(
                        name: "FK_colaborador_estado_civil_id_estado_civil",
                        column: x => x.id_estado_civil,
                        principalTable: "estado_civil",
                        principalColumn: "id_estado_civil");
                    table.ForeignKey(
                        name: "FK_colaborador_forma_ingresso_id_forma_ingresso",
                        column: x => x.id_forma_ingresso,
                        principalTable: "forma_ingresso",
                        principalColumn: "id_forma_ingresso");
                    table.ForeignKey(
                        name: "FK_colaborador_funcao_id_funcao",
                        column: x => x.id_funcao,
                        principalTable: "funcao",
                        principalColumn: "id_funcao");
                    table.ForeignKey(
                        name: "FK_colaborador_unidade_organica_id_unidade_organica",
                        column: x => x.id_unidade_organica,
                        principalTable: "unidade_organica",
                        principalColumn: "id_unidade_organica");
                });

            migrationBuilder.CreateTable(
                name: "opcao_resposta",
                columns: table => new
                {
                    id_opcao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_pergunta = table.Column<int>(type: "int", nullable: false),
                    texto_opcao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ordem = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opcao_resposta", x => x.id_opcao);
                    table.ForeignKey(
                        name: "FK_opcao_resposta_pergunta_inquerito_id_pergunta",
                        column: x => x.id_pergunta,
                        principalTable: "pergunta_inquerito",
                        principalColumn: "id_pergunta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resposta_inquerito",
                columns: table => new
                {
                    id_resposta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_pergunta = table.Column<int>(type: "int", nullable: false),
                    hash_respondente = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    resposta_texto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resposta_numerica = table.Column<int>(type: "int", nullable: true),
                    data_resposta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InqueritoClimaIdInquerito = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resposta_inquerito", x => x.id_resposta);
                    table.ForeignKey(
                        name: "FK_resposta_inquerito_inquerito_clima_InqueritoClimaIdInquerito",
                        column: x => x.InqueritoClimaIdInquerito,
                        principalTable: "inquerito_clima",
                        principalColumn: "id_inquerito");
                    table.ForeignKey(
                        name: "FK_resposta_inquerito_pergunta_inquerito_id_pergunta",
                        column: x => x.id_pergunta,
                        principalTable: "pergunta_inquerito",
                        principalColumn: "id_pergunta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accao_formador",
                columns: table => new
                {
                    id_accao = table.Column<int>(type: "int", nullable: false),
                    id_formador = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accao_formador", x => new { x.id_accao, x.id_formador });
                    table.ForeignKey(
                        name: "FK_accao_formador_accao_formacao_id_accao",
                        column: x => x.id_accao,
                        principalTable: "accao_formacao",
                        principalColumn: "id_accao",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_accao_formador_formador_id_formador",
                        column: x => x.id_formador,
                        principalTable: "formador",
                        principalColumn: "id_formador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "candidatura",
                columns: table => new
                {
                    id_candidatura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_candidato = table.Column<int>(type: "int", nullable: false),
                    id_anuncio = table.Column<int>(type: "int", nullable: false),
                    data_submissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Submetida"),
                    motivo_nao_selecao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candidatura", x => x.id_candidatura);
                    table.ForeignKey(
                        name: "FK_candidatura_anuncio_recrutamento_id_anuncio",
                        column: x => x.id_anuncio,
                        principalTable: "anuncio_recrutamento",
                        principalColumn: "id_anuncio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_candidatura_candidato_id_candidato",
                        column: x => x.id_candidato,
                        principalTable: "candidato",
                        principalColumn: "id_candidato",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "caso_apoio_social",
                columns: table => new
                {
                    id_caso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_colaborador = table.Column<int>(type: "int", nullable: true),
                    tipo_caso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    canal_origem = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    documento_suporte = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    utilizador_registo = table.Column<int>(type: "int", nullable: true),
                    data_criacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_caso_apoio_social", x => x.id_caso);
                    table.ForeignKey(
                        name: "FK_caso_apoio_social_colaborador_id_colaborador",
                        column: x => x.id_colaborador,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador");
                });

            migrationBuilder.CreateTable(
                name: "contrato",
                columns: table => new
                {
                    id_contrato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_colaborador = table.Column<int>(type: "int", nullable: false),
                    id_tipo_contrato = table.Column<int>(type: "int", nullable: false),
                    numero_contrato = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    objecto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remuneracao = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Vigente"),
                    documento_contrato = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    data_assinatura = table.Column<DateOnly>(type: "date", nullable: true),
                    renovacao_anterior = table.Column<int>(type: "int", nullable: true),
                    utilizador_registo = table.Column<int>(type: "int", nullable: true),
                    data_registo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contrato", x => x.id_contrato);
                    table.CheckConstraint("CK_contrato_datas", "[data_fim] IS NULL OR [data_fim] >= [data_inicio]");
                    table.ForeignKey(
                        name: "FK_contrato_colaborador_id_colaborador",
                        column: x => x.id_colaborador,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contrato_contrato_renovacao_anterior",
                        column: x => x.renovacao_anterior,
                        principalTable: "contrato",
                        principalColumn: "id_contrato",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contrato_tipo_contrato_id_tipo_contrato",
                        column: x => x.id_tipo_contrato,
                        principalTable: "tipo_contrato",
                        principalColumn: "id_tipo_contrato",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "estagio",
                columns: table => new
                {
                    id_estagio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome_estagiario = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    documento_identificacao = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    instituicao_ensino = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    tipo_estagio = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    id_unidade_organica = table.Column<int>(type: "int", nullable: false),
                    id_supervisor = table.Column<int>(type: "int", nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    plano_estagio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    declaracao_emitida = table.Column<bool>(type: "bit", nullable: false),
                    utilizador_registo = table.Column<int>(type: "int", nullable: true),
                    data_registo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estagio", x => x.id_estagio);
                    table.ForeignKey(
                        name: "FK_estagio_colaborador_id_supervisor",
                        column: x => x.id_supervisor,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_estagio_unidade_organica_id_unidade_organica",
                        column: x => x.id_unidade_organica,
                        principalTable: "unidade_organica",
                        principalColumn: "id_unidade_organica",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guia_marcha",
                columns: table => new
                {
                    id_guia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_funcionario = table.Column<int>(type: "int", nullable: false),
                    id_projecto = table.Column<int>(type: "int", nullable: true),
                    destino = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    data_partida = table.Column<DateOnly>(type: "date", nullable: false),
                    data_chegada = table.Column<DateOnly>(type: "date", nullable: true),
                    missao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    autorizacao_superior = table.Column<int>(type: "int", nullable: true),
                    utilizador_registo = table.Column<int>(type: "int", nullable: true),
                    data_registo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guia_marcha", x => x.id_guia);
                    table.ForeignKey(
                        name: "FK_guia_marcha_colaborador_autorizacao_superior",
                        column: x => x.autorizacao_superior,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_guia_marcha_colaborador_id_funcionario",
                        column: x => x.id_funcionario,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_guia_marcha_projecto_operacao_id_projecto",
                        column: x => x.id_projecto,
                        principalTable: "projecto_operacao",
                        principalColumn: "id_projecto");
                });

            migrationBuilder.CreateTable(
                name: "historico_colaborador",
                columns: table => new
                {
                    id_historico = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_colaborador = table.Column<int>(type: "int", nullable: false),
                    data_evento = table.Column<DateOnly>(type: "date", nullable: false),
                    tipo_evento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    id_unidade_organica = table.Column<int>(type: "int", nullable: true),
                    id_categoria = table.Column<int>(type: "int", nullable: true),
                    id_carreira = table.Column<int>(type: "int", nullable: true),
                    id_funcao = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historico_colaborador", x => x.id_historico);
                    table.ForeignKey(
                        name: "FK_historico_colaborador_carreira_id_carreira",
                        column: x => x.id_carreira,
                        principalTable: "carreira",
                        principalColumn: "id_carreira");
                    table.ForeignKey(
                        name: "FK_historico_colaborador_categoria_id_categoria",
                        column: x => x.id_categoria,
                        principalTable: "categoria",
                        principalColumn: "id_categoria");
                    table.ForeignKey(
                        name: "FK_historico_colaborador_colaborador_id_colaborador",
                        column: x => x.id_colaborador,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_historico_colaborador_funcao_id_funcao",
                        column: x => x.id_funcao,
                        principalTable: "funcao",
                        principalColumn: "id_funcao");
                    table.ForeignKey(
                        name: "FK_historico_colaborador_unidade_organica_id_unidade_organica",
                        column: x => x.id_unidade_organica,
                        principalTable: "unidade_organica",
                        principalColumn: "id_unidade_organica");
                });

            migrationBuilder.CreateTable(
                name: "inscricao_formacao",
                columns: table => new
                {
                    id_inscricao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_accao = table.Column<int>(type: "int", nullable: false),
                    id_colaborador = table.Column<int>(type: "int", nullable: false),
                    data_inscricao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inscricao_formacao", x => x.id_inscricao);
                    table.ForeignKey(
                        name: "FK_inscricao_formacao_accao_formacao_id_accao",
                        column: x => x.id_accao,
                        principalTable: "accao_formacao",
                        principalColumn: "id_accao",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inscricao_formacao_colaborador_id_colaborador",
                        column: x => x.id_colaborador,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pedido_aprovacao",
                columns: table => new
                {
                    id_pedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_colaborador_solicitante = table.Column<int>(type: "int", nullable: false),
                    id_tipo_pedido = table.Column<int>(type: "int", nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_submissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedido_aprovacao", x => x.id_pedido);
                    table.ForeignKey(
                        name: "FK_pedido_aprovacao_colaborador_id_colaborador_solicitante",
                        column: x => x.id_colaborador_solicitante,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pedido_aprovacao_tipo_pedido_id_tipo_pedido",
                        column: x => x.id_tipo_pedido,
                        principalTable: "tipo_pedido",
                        principalColumn: "id_tipo_pedido",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "registo_ausencia",
                columns: table => new
                {
                    id_registo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_colaborador = table.Column<int>(type: "int", nullable: false),
                    id_tipo_ausencia = table.Column<int>(type: "int", nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: false),
                    motivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    documento_suporte = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    utilizador_registo = table.Column<int>(type: "int", nullable: true),
                    data_registo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registo_ausencia", x => x.id_registo);
                    table.CheckConstraint("CK_ausencia_datas", "[data_fim] >= [data_inicio]");
                    table.ForeignKey(
                        name: "FK_registo_ausencia_colaborador_id_colaborador",
                        column: x => x.id_colaborador,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_registo_ausencia_tipo_ausencia_id_tipo_ausencia",
                        column: x => x.id_tipo_ausencia,
                        principalTable: "tipo_ausencia",
                        principalColumn: "id_tipo_ausencia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "utilizador_sistema",
                columns: table => new
                {
                    id_utilizador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_colaborador = table.Column<int>(type: "int", nullable: true),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    id_perfil = table.Column<int>(type: "int", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Ativo"),
                    ultimo_acesso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    data_criacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utilizador_sistema", x => x.id_utilizador);
                    table.ForeignKey(
                        name: "FK_utilizador_sistema_colaborador_id_colaborador",
                        column: x => x.id_colaborador,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_utilizador_sistema_perfil_acesso_id_perfil",
                        column: x => x.id_perfil,
                        principalTable: "perfil_acesso",
                        principalColumn: "id_perfil",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comunicacao_candidato",
                columns: table => new
                {
                    id_comunicacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_candidatura = table.Column<int>(type: "int", nullable: false),
                    tipo_comunicacao = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    assunto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    corpo_mensagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_envio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comunicacao_candidato", x => x.id_comunicacao);
                    table.ForeignKey(
                        name: "FK_comunicacao_candidato_candidatura_id_candidatura",
                        column: x => x.id_candidatura,
                        principalTable: "candidatura",
                        principalColumn: "id_candidatura",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fase_processo",
                columns: table => new
                {
                    id_fase = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_candidatura = table.Column<int>(type: "int", nullable: false),
                    nome_fase = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: true),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    resultado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    avaliador = table.Column<int>(type: "int", nullable: true),
                    nota = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fase_processo", x => x.id_fase);
                    table.ForeignKey(
                        name: "FK_fase_processo_candidatura_id_candidatura",
                        column: x => x.id_candidatura,
                        principalTable: "candidatura",
                        principalColumn: "id_candidatura",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "nota_acompanhamento",
                columns: table => new
                {
                    id_nota = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_caso = table.Column<int>(type: "int", nullable: false),
                    data_nota = table.Column<DateTime>(type: "datetime2", nullable: false),
                    conteudo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    autor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nota_acompanhamento", x => x.id_nota);
                    table.ForeignKey(
                        name: "FK_nota_acompanhamento_caso_apoio_social_id_caso",
                        column: x => x.id_caso,
                        principalTable: "caso_apoio_social",
                        principalColumn: "id_caso",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "actividade_estagio",
                columns: table => new
                {
                    id_actividade = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_estagio = table.Column<int>(type: "int", nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    data_actividade = table.Column<DateOnly>(type: "date", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actividade_estagio", x => x.id_actividade);
                    table.ForeignKey(
                        name: "FK_actividade_estagio_estagio_id_estagio",
                        column: x => x.id_estagio,
                        principalTable: "estagio",
                        principalColumn: "id_estagio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "avaliacao_estagio",
                columns: table => new
                {
                    id_avaliacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_estagio = table.Column<int>(type: "int", nullable: false),
                    data_avaliacao = table.Column<DateOnly>(type: "date", nullable: false),
                    periodo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    nota = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    pontos_fortes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pontos_melhoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    avaliador = table.Column<int>(type: "int", nullable: true),
                    observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avaliacao_estagio", x => x.id_avaliacao);
                    table.ForeignKey(
                        name: "FK_avaliacao_estagio_estagio_id_estagio",
                        column: x => x.id_estagio,
                        principalTable: "estagio",
                        principalColumn: "id_estagio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "avaliacao_formacao",
                columns: table => new
                {
                    id_avaliacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_inscricao = table.Column<int>(type: "int", nullable: false),
                    data_avaliacao = table.Column<DateOnly>(type: "date", nullable: false),
                    nota = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    avaliador = table.Column<int>(type: "int", nullable: true),
                    certificado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avaliacao_formacao", x => x.id_avaliacao);
                    table.ForeignKey(
                        name: "FK_avaliacao_formacao_inscricao_formacao_id_inscricao",
                        column: x => x.id_inscricao,
                        principalTable: "inscricao_formacao",
                        principalColumn: "id_inscricao",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "aprovacao_pedido",
                columns: table => new
                {
                    id_aprovacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_pedido = table.Column<int>(type: "int", nullable: false),
                    id_aprovador = table.Column<int>(type: "int", nullable: false),
                    nivel_aprovacao = table.Column<int>(type: "int", nullable: false),
                    decisao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    parecer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    data_decisao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aprovacao_pedido", x => x.id_aprovacao);
                    table.ForeignKey(
                        name: "FK_aprovacao_pedido_colaborador_id_aprovador",
                        column: x => x.id_aprovador,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_aprovacao_pedido_pedido_aprovacao_id_pedido",
                        column: x => x.id_pedido,
                        principalTable: "pedido_aprovacao",
                        principalColumn: "id_pedido",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "acto_administrativo",
                columns: table => new
                {
                    id_acto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_colaborador = table.Column<int>(type: "int", nullable: false),
                    id_tipo_acto = table.Column<int>(type: "int", nullable: false),
                    data_acto = table.Column<DateOnly>(type: "date", nullable: false),
                    descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    documento_suporte = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    utilizador_registo = table.Column<int>(type: "int", nullable: true),
                    data_registo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acto_administrativo", x => x.id_acto);
                    table.ForeignKey(
                        name: "FK_acto_administrativo_colaborador_id_colaborador",
                        column: x => x.id_colaborador,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_acto_administrativo_tipo_acto_id_tipo_acto",
                        column: x => x.id_tipo_acto,
                        principalTable: "tipo_acto",
                        principalColumn: "id_tipo_acto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_acto_administrativo_utilizador_sistema_utilizador_registo",
                        column: x => x.utilizador_registo,
                        principalTable: "utilizador_sistema",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "documento",
                columns: table => new
                {
                    id_documento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_colaborador = table.Column<int>(type: "int", nullable: false),
                    id_tipo_documento = table.Column<int>(type: "int", nullable: false),
                    titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ficheiro = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    formato = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    data_upload = table.Column<DateTime>(type: "datetime2", nullable: false),
                    utilizador_upload = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documento", x => x.id_documento);
                    table.ForeignKey(
                        name: "FK_documento_colaborador_id_colaborador",
                        column: x => x.id_colaborador,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_documento_tipo_documento_id_tipo_documento",
                        column: x => x.id_tipo_documento,
                        principalTable: "tipo_documento",
                        principalColumn: "id_tipo_documento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_documento_utilizador_sistema_utilizador_upload",
                        column: x => x.utilizador_upload,
                        principalTable: "utilizador_sistema",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "exportacao",
                columns: table => new
                {
                    id_exportacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_utilizador = table.Column<int>(type: "int", nullable: false),
                    tipo_exportacao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    modulo_origem = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    filtros_aplicados = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    data_exportacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ficheiro_gerado = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exportacao", x => x.id_exportacao);
                    table.ForeignKey(
                        name: "FK_exportacao_utilizador_sistema_id_utilizador",
                        column: x => x.id_utilizador,
                        principalTable: "utilizador_sistema",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "log_auditoria",
                columns: table => new
                {
                    id_log = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_colaborador = table.Column<int>(type: "int", nullable: true),
                    id_utilizador = table.Column<int>(type: "int", nullable: true),
                    tabela_afetada = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    operacao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    dados_anteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dados_posteriores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    data_hora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ip_address = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_log_auditoria", x => x.id_log);
                    table.ForeignKey(
                        name: "FK_log_auditoria_colaborador_id_colaborador",
                        column: x => x.id_colaborador,
                        principalTable: "colaborador",
                        principalColumn: "id_colaborador",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_log_auditoria_utilizador_sistema_id_utilizador",
                        column: x => x.id_utilizador,
                        principalTable: "utilizador_sistema",
                        principalColumn: "id_utilizador",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_accao_plano",
                table: "accao_formacao",
                column: "id_plano");

            migrationBuilder.CreateIndex(
                name: "IX_accao_formador_id_formador",
                table: "accao_formador",
                column: "id_formador");

            migrationBuilder.CreateIndex(
                name: "idx_actest_estag",
                table: "actividade_estagio",
                column: "id_estagio");

            migrationBuilder.CreateIndex(
                name: "idx_acto_colab",
                table: "acto_administrativo",
                column: "id_colaborador");

            migrationBuilder.CreateIndex(
                name: "IX_acto_administrativo_id_tipo_acto",
                table: "acto_administrativo",
                column: "id_tipo_acto");

            migrationBuilder.CreateIndex(
                name: "IX_acto_administrativo_utilizador_registo",
                table: "acto_administrativo",
                column: "utilizador_registo");

            migrationBuilder.CreateIndex(
                name: "IX_anuncio_recrutamento_id_unidade_organica",
                table: "anuncio_recrutamento",
                column: "id_unidade_organica");

            migrationBuilder.CreateIndex(
                name: "idx_aprov_pedido",
                table: "aprovacao_pedido",
                column: "id_pedido");

            migrationBuilder.CreateIndex(
                name: "IX_aprovacao_pedido_id_aprovador",
                table: "aprovacao_pedido",
                column: "id_aprovador");

            migrationBuilder.CreateIndex(
                name: "idx_avales_estag",
                table: "avaliacao_estagio",
                column: "id_estagio");

            migrationBuilder.CreateIndex(
                name: "IX_avaliacao_formacao_id_inscricao",
                table: "avaliacao_formacao",
                column: "id_inscricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_candidato_id_nivel_academico",
                table: "candidato",
                column: "id_nivel_academico");

            migrationBuilder.CreateIndex(
                name: "idx_cand_anuncio",
                table: "candidatura",
                column: "id_anuncio");

            migrationBuilder.CreateIndex(
                name: "idx_cand_candidato",
                table: "candidatura",
                column: "id_candidato");

            migrationBuilder.CreateIndex(
                name: "idx_cand_estado",
                table: "candidatura",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "UQ_candidatura",
                table: "candidatura",
                columns: new[] { "id_candidato", "id_anuncio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_caso_colab",
                table: "caso_apoio_social",
                column: "id_colaborador");

            migrationBuilder.CreateIndex(
                name: "idx_caso_estado",
                table: "caso_apoio_social",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "idx_colab_estado",
                table: "colaborador",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "idx_colab_nuit",
                table: "colaborador",
                column: "nuit");

            migrationBuilder.CreateIndex(
                name: "idx_colab_unidade",
                table: "colaborador",
                column: "id_unidade_organica");

            migrationBuilder.CreateIndex(
                name: "IX_colaborador_id_carreira",
                table: "colaborador",
                column: "id_carreira");

            migrationBuilder.CreateIndex(
                name: "IX_colaborador_id_categoria",
                table: "colaborador",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_colaborador_id_estado_civil",
                table: "colaborador",
                column: "id_estado_civil");

            migrationBuilder.CreateIndex(
                name: "IX_colaborador_id_forma_ingresso",
                table: "colaborador",
                column: "id_forma_ingresso");

            migrationBuilder.CreateIndex(
                name: "IX_colaborador_id_funcao",
                table: "colaborador",
                column: "id_funcao");

            migrationBuilder.CreateIndex(
                name: "idx_comunic_candidatura",
                table: "comunicacao_candidato",
                column: "id_candidatura");

            migrationBuilder.CreateIndex(
                name: "idx_ctr_colab",
                table: "contrato",
                column: "id_colaborador");

            migrationBuilder.CreateIndex(
                name: "idx_ctr_datafim",
                table: "contrato",
                column: "data_fim");

            migrationBuilder.CreateIndex(
                name: "idx_ctr_estado",
                table: "contrato",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_contrato_id_tipo_contrato",
                table: "contrato",
                column: "id_tipo_contrato");

            migrationBuilder.CreateIndex(
                name: "IX_contrato_renovacao_anterior",
                table: "contrato",
                column: "renovacao_anterior");

            migrationBuilder.CreateIndex(
                name: "idx_doc_colab",
                table: "documento",
                column: "id_colaborador");

            migrationBuilder.CreateIndex(
                name: "IX_documento_id_tipo_documento",
                table: "documento",
                column: "id_tipo_documento");

            migrationBuilder.CreateIndex(
                name: "IX_documento_utilizador_upload",
                table: "documento",
                column: "utilizador_upload");

            migrationBuilder.CreateIndex(
                name: "idx_estag_supervisor",
                table: "estagio",
                column: "id_supervisor");

            migrationBuilder.CreateIndex(
                name: "idx_estag_unidade",
                table: "estagio",
                column: "id_unidade_organica");

            migrationBuilder.CreateIndex(
                name: "idx_export_util",
                table: "exportacao",
                column: "id_utilizador");

            migrationBuilder.CreateIndex(
                name: "idx_fase_candidatura",
                table: "fase_processo",
                column: "id_candidatura");

            migrationBuilder.CreateIndex(
                name: "idx_gm_datas",
                table: "guia_marcha",
                columns: new[] { "data_partida", "data_chegada" });

            migrationBuilder.CreateIndex(
                name: "idx_gm_estado",
                table: "guia_marcha",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "idx_gm_func",
                table: "guia_marcha",
                column: "id_funcionario");

            migrationBuilder.CreateIndex(
                name: "IX_guia_marcha_autorizacao_superior",
                table: "guia_marcha",
                column: "autorizacao_superior");

            migrationBuilder.CreateIndex(
                name: "IX_guia_marcha_id_projecto",
                table: "guia_marcha",
                column: "id_projecto");

            migrationBuilder.CreateIndex(
                name: "idx_hist_colab",
                table: "historico_colaborador",
                column: "id_colaborador");

            migrationBuilder.CreateIndex(
                name: "IX_historico_colaborador_id_carreira",
                table: "historico_colaborador",
                column: "id_carreira");

            migrationBuilder.CreateIndex(
                name: "IX_historico_colaborador_id_categoria",
                table: "historico_colaborador",
                column: "id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_historico_colaborador_id_funcao",
                table: "historico_colaborador",
                column: "id_funcao");

            migrationBuilder.CreateIndex(
                name: "IX_historico_colaborador_id_unidade_organica",
                table: "historico_colaborador",
                column: "id_unidade_organica");

            migrationBuilder.CreateIndex(
                name: "idx_insc_accao",
                table: "inscricao_formacao",
                column: "id_accao");

            migrationBuilder.CreateIndex(
                name: "idx_insc_colab",
                table: "inscricao_formacao",
                column: "id_colaborador");

            migrationBuilder.CreateIndex(
                name: "UQ_inscricao",
                table: "inscricao_formacao",
                columns: new[] { "id_accao", "id_colaborador" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_audit_data",
                table: "log_auditoria",
                column: "data_hora");

            migrationBuilder.CreateIndex(
                name: "idx_audit_tabela",
                table: "log_auditoria",
                column: "tabela_afetada");

            migrationBuilder.CreateIndex(
                name: "IX_log_auditoria_id_colaborador",
                table: "log_auditoria",
                column: "id_colaborador");

            migrationBuilder.CreateIndex(
                name: "IX_log_auditoria_id_utilizador",
                table: "log_auditoria",
                column: "id_utilizador");

            migrationBuilder.CreateIndex(
                name: "idx_nota_caso",
                table: "nota_acompanhamento",
                column: "id_caso");

            migrationBuilder.CreateIndex(
                name: "idx_opcao_pergunta",
                table: "opcao_resposta",
                column: "id_pergunta");

            migrationBuilder.CreateIndex(
                name: "idx_ped_solicitante",
                table: "pedido_aprovacao",
                column: "id_colaborador_solicitante");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_aprovacao_id_tipo_pedido",
                table: "pedido_aprovacao",
                column: "id_tipo_pedido");

            migrationBuilder.CreateIndex(
                name: "idx_perg_inq",
                table: "pergunta_inquerito",
                column: "id_inquerito");

            migrationBuilder.CreateIndex(
                name: "idx_perm_perfil",
                table: "permissao",
                column: "id_perfil");

            migrationBuilder.CreateIndex(
                name: "UQ_permissao",
                table: "permissao",
                columns: new[] { "id_perfil", "modulo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_aus_colab",
                table: "registo_ausencia",
                column: "id_colaborador");

            migrationBuilder.CreateIndex(
                name: "idx_aus_estado",
                table: "registo_ausencia",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_registo_ausencia_id_tipo_ausencia",
                table: "registo_ausencia",
                column: "id_tipo_ausencia");

            migrationBuilder.CreateIndex(
                name: "idx_resp_perg",
                table: "resposta_inquerito",
                column: "id_pergunta");

            migrationBuilder.CreateIndex(
                name: "IX_resposta_inquerito_InqueritoClimaIdInquerito",
                table: "resposta_inquerito",
                column: "InqueritoClimaIdInquerito");

            migrationBuilder.CreateIndex(
                name: "idx_uo_pai",
                table: "unidade_organica",
                column: "id_unidade_pai");

            migrationBuilder.CreateIndex(
                name: "idx_util_perfil",
                table: "utilizador_sistema",
                column: "id_perfil");

            migrationBuilder.CreateIndex(
                name: "idx_util_username",
                table: "utilizador_sistema",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_utilizador_sistema_id_colaborador",
                table: "utilizador_sistema",
                column: "id_colaborador",
                unique: true,
                filter: "[id_colaborador] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accao_formador");

            migrationBuilder.DropTable(
                name: "actividade_estagio");

            migrationBuilder.DropTable(
                name: "acto_administrativo");

            migrationBuilder.DropTable(
                name: "aprovacao_pedido");

            migrationBuilder.DropTable(
                name: "avaliacao_estagio");

            migrationBuilder.DropTable(
                name: "avaliacao_formacao");

            migrationBuilder.DropTable(
                name: "comunicacao_candidato");

            migrationBuilder.DropTable(
                name: "contrato");

            migrationBuilder.DropTable(
                name: "documento");

            migrationBuilder.DropTable(
                name: "exportacao");

            migrationBuilder.DropTable(
                name: "fase_processo");

            migrationBuilder.DropTable(
                name: "guia_marcha");

            migrationBuilder.DropTable(
                name: "historico_colaborador");

            migrationBuilder.DropTable(
                name: "indicador_calculado");

            migrationBuilder.DropTable(
                name: "log_auditoria");

            migrationBuilder.DropTable(
                name: "nota_acompanhamento");

            migrationBuilder.DropTable(
                name: "opcao_resposta");

            migrationBuilder.DropTable(
                name: "permissao");

            migrationBuilder.DropTable(
                name: "registo_ausencia");

            migrationBuilder.DropTable(
                name: "resposta_inquerito");

            migrationBuilder.DropTable(
                name: "formador");

            migrationBuilder.DropTable(
                name: "tipo_acto");

            migrationBuilder.DropTable(
                name: "pedido_aprovacao");

            migrationBuilder.DropTable(
                name: "estagio");

            migrationBuilder.DropTable(
                name: "inscricao_formacao");

            migrationBuilder.DropTable(
                name: "tipo_contrato");

            migrationBuilder.DropTable(
                name: "tipo_documento");

            migrationBuilder.DropTable(
                name: "candidatura");

            migrationBuilder.DropTable(
                name: "projecto_operacao");

            migrationBuilder.DropTable(
                name: "utilizador_sistema");

            migrationBuilder.DropTable(
                name: "caso_apoio_social");

            migrationBuilder.DropTable(
                name: "tipo_ausencia");

            migrationBuilder.DropTable(
                name: "pergunta_inquerito");

            migrationBuilder.DropTable(
                name: "tipo_pedido");

            migrationBuilder.DropTable(
                name: "accao_formacao");

            migrationBuilder.DropTable(
                name: "anuncio_recrutamento");

            migrationBuilder.DropTable(
                name: "candidato");

            migrationBuilder.DropTable(
                name: "perfil_acesso");

            migrationBuilder.DropTable(
                name: "colaborador");

            migrationBuilder.DropTable(
                name: "inquerito_clima");

            migrationBuilder.DropTable(
                name: "plano_formacao");

            migrationBuilder.DropTable(
                name: "nivel_academico");

            migrationBuilder.DropTable(
                name: "carreira");

            migrationBuilder.DropTable(
                name: "categoria");

            migrationBuilder.DropTable(
                name: "estado_civil");

            migrationBuilder.DropTable(
                name: "forma_ingresso");

            migrationBuilder.DropTable(
                name: "funcao");

            migrationBuilder.DropTable(
                name: "unidade_organica");
        }
    }
}
