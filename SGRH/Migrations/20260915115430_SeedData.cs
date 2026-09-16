using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGRH.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "carreira",
                columns: new[] { "id_carreira", "classe_final", "classe_inicial", "descricao", "nome" },
                values: new object[,]
                {
                    { 1, null, null, "Funções de direcção e confiança política", "Carreira de Direcção e Confiança" },
                    { 2, null, null, "Funções técnicas de estatística", "Carreira de Estatística" },
                    { 3, null, null, "Funções técnicas de informática", "Carreira de Informática" },
                    { 4, null, null, "Funções administrativas e de gestão", "Carreira Administrativa" },
                    { 5, null, null, "Funções de investigação e pesquisa", "Carreira de Investigação Científica" },
                    { 6, null, null, "Regime especial sem diferenciação de carreira", "Regime Especial Não Diferenciado" }
                });

            migrationBuilder.InsertData(
                table: "categoria",
                columns: new[] { "id_categoria", "codigo", "descricao", "nivel" },
                values: new object[,]
                {
                    { 1, null, "Assessor Principal", 1 },
                    { 2, null, "Primeiro Assessor", 2 },
                    { 3, null, "Assessor", 3 },
                    { 4, null, "Técnico Superior Principal", 4 },
                    { 5, null, "Técnico Superior de 1ª Classe", 5 },
                    { 6, null, "Técnico Superior de 2ª Classe", 6 },
                    { 7, null, "Especialista Principal", 7 },
                    { 8, null, "Especialista de 1ª Classe", 8 },
                    { 9, null, "Especialista de 2ª Classe", 9 },
                    { 10, null, "Técnico de 1ª Classe", 10 },
                    { 11, null, "Técnico de 2ª Classe", 11 },
                    { 12, null, "Técnico de 3ª Classe", 12 },
                    { 13, null, "Técnico Médio Principal", 13 },
                    { 14, null, "Técnico Médio de 1ª Classe", 14 },
                    { 15, null, "Técnico Médio de 2ª Classe", 15 },
                    { 16, null, "Técnico Médio de 3ª Classe", 16 },
                    { 17, null, "Oficial Administrativo Principal", 17 },
                    { 18, null, "1º Oficial Administrativo", 18 },
                    { 19, null, "2º Oficial Administrativo", 19 },
                    { 20, null, "3º Oficial Administrativo", 20 },
                    { 21, null, "Auxiliar Administrativo", 21 },
                    { 22, null, "Agente de Serviço", 22 }
                });

            migrationBuilder.InsertData(
                table: "estado_civil",
                columns: new[] { "id_estado_civil", "nome" },
                values: new object[,]
                {
                    { 1, "Solteiro(a)" },
                    { 2, "Casado(a)" },
                    { 3, "Divorciado(a)" },
                    { 4, "Viúvo(a)" },
                    { 5, "União de Facto" }
                });

            migrationBuilder.InsertData(
                table: "forma_ingresso",
                columns: new[] { "id_forma_ingresso", "nome" },
                values: new object[,]
                {
                    { 1, "Concurso Público" },
                    { 2, "Nomeação" },
                    { 3, "Contratação a Termo Certo" },
                    { 4, "Contratação a Termo Indeterminado" },
                    { 5, "Destacamento" },
                    { 6, "Mobilidade" }
                });

            migrationBuilder.InsertData(
                table: "funcao",
                columns: new[] { "id_funcao", "descricao", "nome", "remuneracao_base" },
                values: new object[,]
                {
                    { 1, "Responsável máximo pela gestão do INE", "Director Geral", null },
                    { 2, "Adjunto do Director Geral", "Director Geral Adjunto", null },
                    { 3, "Responsável por um departamento", "Chefe de Departamento", null },
                    { 4, "Responsável por uma secção", "Chefe de Secção", null },
                    { 5, "Profissional de estatística", "Estatístico", null },
                    { 6, "Técnico em operações estatísticas", "Técnico de Estatística", null },
                    { 7, "Análise e tratamento de dados", "Analista de Dados", null },
                    { 8, "Elaboração de mapas e cartografia", "Cartógrafo", null },
                    { 9, "Desenvolvimento de software", "Programador", null },
                    { 10, "Suporte e manutenção de TI", "Técnico de Informática", null },
                    { 11, "Gestão de sistemas informáticos", "Administrador de Sistemas", null },
                    { 12, "Gestão contabilística e financeira", "Contabilista", null },
                    { 13, "Funções administrativas", "Técnico de Administração", null },
                    { 14, "Apoio administrativo", "Assistente Administrativo", null },
                    { 15, "Apoio à direcção", "Secretário de Direcção", null },
                    { 16, "Serviço de transporte", "Motorista", null },
                    { 17, "Vigilância e segurança", "Agente de Segurança", null },
                    { 18, "Serviços de limpeza e conservação", "Auxiliar de Limpeza", null }
                });

            migrationBuilder.InsertData(
                table: "tipo_contrato",
                columns: new[] { "id_tipo_contrato", "descricao", "duracao_maxima_meses", "nome" },
                values: new object[,]
                {
                    { 1, "Contrato sem termo final", null, "Contrato por Tempo Indeterminado" },
                    { 2, "Contrato com data de fim definida", 24, "Contrato a Termo Certo" },
                    { 3, "Contrato para substituição temporária", 12, "Contrato a Termo Incerto" },
                    { 4, "Prestação de serviços especializados", 12, "Contrato de Prestação de Serviços" },
                    { 5, "Nomeação em funções públicas", null, "Nomeação" },
                    { 6, "Comissão temporária de serviço", 12, "Comissão de Serviço" }
                });

            migrationBuilder.InsertData(
                table: "unidade_organica",
                columns: new[] { "id_unidade_organica", "estado", "id_unidade_pai", "nome", "sigla", "tipo" },
                values: new object[,]
                {
                    { 1, "Ativa", null, "Presidência do INE", "PRES", "Departamento" },
                    { 2, "Ativa", null, "Departamento de Apoio ao Director Geral", "DADG", "Departamento" },
                    { 3, "Ativa", null, "Departamento de Recursos Humanos, Administração e Finanças", "DRHAF", "Departamento" },
                    { 4, "Ativa", null, "Departamento de Tecnologias de Informação", "DTI", "Departamento" },
                    { 5, "Ativa", null, "Departamento de Contas Nacionais e Coordenação Estatística", "DCNCE", "Departamento" },
                    { 6, "Ativa", null, "Departamento de Estatísticas Económicas e Financeiras", "DEEF", "Departamento" },
                    { 7, "Ativa", null, "Departamento de Estatísticas Demográficas e Sociais", "DEDS", "Departamento" },
                    { 8, "Ativa", null, "Departamento de Censos e Inquéritos Especiais", "DCIE", "Departamento" },
                    { 9, "Ativa", null, "Departamento de Informação e Difusão", "DID", "Departamento" },
                    { 10, "Ativa", null, "Escola Nacional de Estatística", "ENE", "Escola" },
                    { 11, "Ativa", null, "Delegação Provincial - Niassa", "DP-NIA", "Delegação" },
                    { 12, "Ativa", null, "Delegação Provincial - Cabo Delgado", "DP-CDL", "Delegação" },
                    { 13, "Ativa", null, "Delegação Provincial - Nampula", "DP-NAM", "Delegação" },
                    { 14, "Ativa", null, "Delegação Provincial - Zambézia", "DP-ZAM", "Delegação" },
                    { 15, "Ativa", null, "Delegação Provincial - Tete", "DP-TET", "Delegação" },
                    { 16, "Ativa", null, "Delegação Provincial - Manica", "DP-MAN", "Delegação" },
                    { 17, "Ativa", null, "Delegação Provincial - Sofala", "DP-SOF", "Delegação" },
                    { 18, "Ativa", null, "Delegação Provincial - Inhambane", "DP-INH", "Delegação" },
                    { 19, "Ativa", null, "Delegação Provincial - Gaza", "DP-GAZ", "Delegação" },
                    { 20, "Ativa", null, "Delegação Provincial - Maputo Província", "DP-MP", "Delegação" },
                    { 21, "Ativa", null, "Delegação Provincial - Maputo Cidade", "DP-MC", "Delegação" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "carreira",
                keyColumn: "id_carreira",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "carreira",
                keyColumn: "id_carreira",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "carreira",
                keyColumn: "id_carreira",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "carreira",
                keyColumn: "id_carreira",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "carreira",
                keyColumn: "id_carreira",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "carreira",
                keyColumn: "id_carreira",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "categoria",
                keyColumn: "id_categoria",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "estado_civil",
                keyColumn: "id_estado_civil",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "estado_civil",
                keyColumn: "id_estado_civil",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "estado_civil",
                keyColumn: "id_estado_civil",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "estado_civil",
                keyColumn: "id_estado_civil",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "estado_civil",
                keyColumn: "id_estado_civil",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "forma_ingresso",
                keyColumn: "id_forma_ingresso",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "forma_ingresso",
                keyColumn: "id_forma_ingresso",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "forma_ingresso",
                keyColumn: "id_forma_ingresso",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "forma_ingresso",
                keyColumn: "id_forma_ingresso",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "forma_ingresso",
                keyColumn: "id_forma_ingresso",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "forma_ingresso",
                keyColumn: "id_forma_ingresso",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "funcao",
                keyColumn: "id_funcao",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "tipo_contrato",
                keyColumn: "id_tipo_contrato",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "tipo_contrato",
                keyColumn: "id_tipo_contrato",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "tipo_contrato",
                keyColumn: "id_tipo_contrato",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "tipo_contrato",
                keyColumn: "id_tipo_contrato",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "tipo_contrato",
                keyColumn: "id_tipo_contrato",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "tipo_contrato",
                keyColumn: "id_tipo_contrato",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "unidade_organica",
                keyColumn: "id_unidade_organica",
                keyValue: 21);
        }
    }
}
