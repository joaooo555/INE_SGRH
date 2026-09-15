/* ============================================
   SGRH - App Principal (jQuery + Mock Data)
   ============================================ */

// ---- Mock Data: Colaboradores (INE Moçambique) ----
const defaultColaboradores = [
    { id: 1, nome: "Joaquim Alberto Chissano", nuit: "4455667788990N", dataNascimento: "1970-01-05", sexo: "M", estadoCivil: "Casado", nacionalidade: "Moçambicana", contacto: "+258 85 445 5667", email: "joaquim.chissano@ine.gov.mz", unidade: "Presidência do INE", categoria: "Assessor Principal", carreira: "Carreira de Direcção e Confiança", funcao: "Director Geral", dataIngresso: "2000-04-10", formaIngresso: "Nomeação", estado: "Ativo", foto: "JC", contrato: { tipo: "Contrato por Tempo Indeterminado", dataInicio: "2000-04-10", dataFim: "", remuneracao: "120000", estado: "Vigente" }, documentos: ["BI_Joaquim.pdf", "Despacho_Nomeacao.pdf"], observacoes: "Quadro sénior com mais de 20 anos de serviço no INE." },
    { id: 2, nome: "Hélio Biliti Chichava", nuit: "2233445566778L", dataNascimento: "1976-10-12", sexo: "M", estadoCivil: "Viúvo", nacionalidade: "Moçambicana", contacto: "+258 86 223 3445", email: "helio.chichava@ine.gov.mz", unidade: "Presidência do INE", categoria: "Primeiro Assessor", carreira: "Carreira de Direcção e Confiança", funcao: "Director Geral Adjunto", dataIngresso: "2003-01-15", formaIngresso: "Nomeação", estado: "Ativo", foto: "HC", contrato: { tipo: "Contrato por Tempo Indeterminado", dataInicio: "2003-01-15", dataFim: "", remuneracao: "105000", estado: "Vigente" }, documentos: ["BI_Helio.pdf"], observacoes: "" },
    { id: 3, nome: "Ana Maria Fernandes", nuit: "1234567890123A", dataNascimento: "1985-03-15", sexo: "F", estadoCivil: "Casado", nacionalidade: "Moçambicana", contacto: "+258 84 123 4567", email: "ana.fernandes@ine.gov.mz", unidade: "Departamento de Recursos Humanos, Administração e Finanças (DRHAF)", categoria: "Técnico Superior de 1ª Classe", carreira: "Carreira Administrativa", funcao: "Chefe de Departamento", dataIngresso: "2010-06-01", formaIngresso: "Concurso Público", estado: "Ativo", foto: "AF", contrato: { tipo: "Contrato a Termo Certo", dataInicio: "2024-01-01", dataFim: "2026-12-31", remuneracao: "85000", estado: "Vigente" }, documentos: ["Certificado_Licenciatura.pdf", "Contrato_2024.docx"], observacoes: "" },
    { id: 4, nome: "Carlos Alberto Machava", nuit: "9876543210987B", dataNascimento: "1978-07-22", sexo: "M", estadoCivil: "Casado", nacionalidade: "Moçambicana", contacto: "+258 85 234 5678", email: "carlos.machava@ine.gov.mz", unidade: "Departamento de Tecnologias de Informação (DTI)", categoria: "Técnico Superior de 1ª Classe", carreira: "Carreira de Informática", funcao: "Chefe de Departamento", dataIngresso: "2008-03-15", formaIngresso: "Nomeação", estado: "Ativo", foto: "CM", contrato: { tipo: "Contrato por Tempo Indeterminado", dataInicio: "2023-06-01", dataFim: "", remuneracao: "95000", estado: "Vigente" }, documentos: ["Certificacoes_TI.pdf"], observacoes: "" },
    { id: 5, nome: "Fátima Zacarias Tembe", nuit: "5678901234567C", dataNascimento: "1990-11-08", sexo: "F", estadoCivil: "Solteiro", nacionalidade: "Moçambicana", contacto: "+258 84 345 6789", email: "fatima.tembe@ine.gov.mz", unidade: "Departamento de Contas Nacionais e Coordenação Estatística (DCNCE)", categoria: "Especialista de 1ª Classe", carreira: "Carreira de Estatística", funcao: "Estatístico", dataIngresso: "2015-01-10", formaIngresso: "Concurso Público", estado: "Ativo", foto: "FT", contrato: { tipo: "Contrato a Termo Certo", dataInicio: "2024-03-01", dataFim: "2026-02-28", remuneracao: "65000", estado: "Vigente" }, documentos: ["BI_Fatima.pdf"], observacoes: "" },
    { id: 6, nome: "José Manuel Sitoe", nuit: "3456789012345D", dataNascimento: "1982-05-30", sexo: "M", estadoCivil: "Divorciado", nacionalidade: "Moçambicana", contacto: "+258 86 456 7890", email: "jose.sitoe@ine.gov.mz", unidade: "Departamento de Estatísticas Económicas e Financeiras (DEEF)", categoria: "Técnico de 1ª Classe", carreira: "Carreira de Estatística", funcao: "Técnico de Estatística", dataIngresso: "2012-09-20", formaIngresso: "Contratação a Termo Indeterminado", estado: "Ativo", foto: "JS", contrato: { tipo: "Contrato por Tempo Indeterminado", dataInicio: "2022-09-01", dataFim: "", remuneracao: "55000", estado: "Vigente" }, documentos: [], observacoes: "" },
    { id: 7, nome: "Mariana Inácio Mondlane", nuit: "7890123456789E", dataNascimento: "1995-01-18", sexo: "F", estadoCivil: "Solteiro", nacionalidade: "Moçambicana", contacto: "+258 84 567 8901", email: "mariana.mondlane@ine.gov.mz", unidade: "Departamento de Tecnologias de Informação (DTI)", categoria: "Técnico de 2ª Classe", carreira: "Carreira de Informática", funcao: "Programador", dataIngresso: "2020-02-01", formaIngresso: "Concurso Público", estado: "Ativo", foto: "MM", contrato: { tipo: "Contrato a Termo Certo", dataInicio: "2025-02-01", dataFim: "2026-09-26", remuneracao: "48000", estado: "Vigente" }, documentos: ["Certificado_Programacao.pdf"], observacoes: "" },
    { id: 8, nome: "Pedro Henrique Cossa", nuit: "4567890123456F", dataNascimento: "1975-12-03", sexo: "M", estadoCivil: "Casado", nacionalidade: "Moçambicana", contacto: "+258 85 678 9012", email: "pedro.cossa@ine.gov.mz", unidade: "Departamento de Apoio ao Director Geral (DADG)", categoria: "Técnico Superior de 1ª Classe", carreira: "Carreira Administrativa", funcao: "Chefe de Departamento", dataIngresso: "2005-08-10", formaIngresso: "Nomeação", estado: "Ativo", foto: "PC", contrato: { tipo: "Contrato por Tempo Indeterminado", dataInicio: "2024-08-15", dataFim: "", remuneracao: "78000", estado: "Vigente" }, documentos: [], observacoes: "" },
    { id: 9, nome: "Rosa Amélia Banze", nuit: "2345678901234G", dataNascimento: "1988-06-25", sexo: "F", estadoCivil: "Casado", nacionalidade: "Moçambicana", contacto: "+258 84 789 0123", email: "rosa.banze@ine.gov.mz", unidade: "Departamento de Estatísticas Demográficas e Sociais (DEDS)", categoria: "Especialista de 1ª Classe", carreira: "Carreira de Estatística", funcao: "Estatístico", dataIngresso: "2016-04-05", formaIngresso: "Concurso Público", estado: "Ativo", foto: "RB", contrato: { tipo: "Contrato a Termo Certo", dataInicio: "2023-04-01", dataFim: "2026-09-19", remuneracao: "52000", estado: "Vigente" }, documentos: [], observacoes: "" },
    { id: 10, nome: "Tomás Ngwenya Dlamini", nuit: "6789012345678H", dataNascimento: "1992-09-14", sexo: "M", estadoCivil: "Solteiro", nacionalidade: "Moçambicana", contacto: "+258 86 890 1234", email: "tomas.dlamini@ine.gov.mz", unidade: "Departamento de Censos e Inquéritos Especiais (DCIE)", categoria: "Técnico de 2ª Classe", carreira: "Carreira de Estatística", funcao: "Cartógrafo", dataIngresso: "2019-07-15", formaIngresso: "Contratação a Termo Certo", estado: "Ativo", foto: "TD", contrato: { tipo: "Contrato a Termo Certo", dataInicio: "2024-07-01", dataFim: "2026-06-30", remuneracao: "42000", estado: "Vigente" }, documentos: [], observacoes: "" },
    { id: 11, nome: "Vanessa Lourenço Matusse", nuit: "8901234567890I", dataNascimento: "1993-04-07", sexo: "F", estadoCivil: "Solteiro", nacionalidade: "Moçambicana", contacto: "+258 84 901 2345", email: "vanessa.matusse@ine.gov.mz", unidade: "Departamento de Informação e Difusão (DID)", categoria: "Técnico de 2ª Classe", carreira: "Carreira Administrativa", funcao: "Assistente Administrativo", dataIngresso: "2021-01-10", formaIngresso: "Concurso Público", estado: "Ativo", foto: "VM", contrato: { tipo: "Contrato a Termo Certo", dataInicio: "2025-01-10", dataFim: "2027-01-09", remuneracao: "45000", estado: "Vigente" }, documentos: [], observacoes: "" },
    { id: 12, nome: "Wilson António Nhaca", nuit: "0123456789012J", dataNascimento: "1980-08-19", sexo: "M", estadoCivil: "Casado", nacionalidade: "Moçambicana", contacto: "+258 85 012 3456", email: "wilson.nhaca@ine.gov.mz", unidade: "Departamento de Recursos Humanos, Administração e Finanças (DRHAF)", categoria: "Técnico Superior Principal", carreira: "Carreira Administrativa", funcao: "Contabilista", dataIngresso: "2007-11-20", formaIngresso: "Nomeação", estado: "Ativo", foto: "WN", contrato: { tipo: "Contrato por Tempo Indeterminado", dataInicio: "2021-11-20", dataFim: "", remuneracao: "88000", estado: "Vigente" }, documentos: [], observacoes: "" },
    { id: 13, nome: "Graça Machel Nguenha", nuit: "1122334455667K", dataNascimento: "1991-02-28", sexo: "F", estadoCivil: "Casado", nacionalidade: "Moçambicana", contacto: "+258 84 112 2334", email: "graca.nguenha@ine.gov.mz", unidade: "Delegação Provincial - Nampula", categoria: "Técnico de 1ª Classe", carreira: "Carreira de Estatística", funcao: "Técnico de Estatística", dataIngresso: "2018-03-01", formaIngresso: "Concurso Público", estado: "Ativo", foto: "GN", contrato: { tipo: "Contrato por Tempo Indeterminado", dataInicio: "2018-03-01", dataFim: "", remuneracao: "56000", estado: "Vigente" }, documentos: [], observacoes: "" },
    { id: 14, nome: "Isabel Torres Macamo", nuit: "3344556677889M", dataNascimento: "1994-12-20", sexo: "F", estadoCivil: "Solteiro", nacionalidade: "Moçambicana", contacto: "+258 84 334 4556", email: "isabel.macamo@ine.gov.mz", unidade: "Escola Nacional de Estatística (ENE)", categoria: "Técnico de 2ª Classe", carreira: "Carreira de Investigação Científica", funcao: "Analista de Dados", dataIngresso: "2022-06-01", formaIngresso: "Contratação a Termo Certo", estado: "Ativo", foto: "IM", contrato: { tipo: "Contrato a Termo Certo", dataInicio: "2022-06-01", dataFim: "2026-12-31", remuneracao: "50000", estado: "Vigente" }, documentos: [], observacoes: "" },
    { id: 15, nome: "Lúcia Francisca Mabunda", nuit: "5566778899001O", dataNascimento: "1987-07-30", sexo: "F", estadoCivil: "Casado", nacionalidade: "Moçambicana", contacto: "+258 84 556 6778", email: "lucia.mabunda@ine.gov.mz", unidade: "Delegação Provincial - Maputo Cidade", categoria: "Técnico Médio de 1ª Classe", carreira: "Regime Especial Não Diferenciado", funcao: "Assistente Administrativo", dataIngresso: "2017-09-01", formaIngresso: "Concurso Público", estado: "Ativo", foto: "LM", contrato: { tipo: "Contrato por Tempo Indeterminado", dataInicio: "2017-09-01", dataFim: "", remuneracao: "38000", estado: "Vigente" }, documentos: [], observacoes: "" },
];

function getColaboradoresStorage() {
    try {
        var raw = localStorage.getItem('sgrh_colaboradores');
        if (raw) {
            return JSON.parse(raw);
        }
    } catch (e) {}
    return defaultColaboradores;
}

function saveColaboradoresStorage(arr) {
    try {
        localStorage.setItem('sgrh_colaboradores', JSON.stringify(arr));
    } catch (e) {}
}

var colaboradores = getColaboradoresStorage();

// ---- Mock Data: Contratos ----
const contratos = [
    { id: 1, colaborador: "Ana Maria Fernandes", numero: "CTR-2024/001", tipo: "Contrato a Termo Certo", dataInicio: "2024-01-01", dataFim: "2026-12-31", remuneracao: 85000, estado: "Vigente" },
    { id: 2, colaborador: "Carlos Alberto Machava", numero: "CTR-2023/015", tipo: "Contrato por Tempo Indeterminado", dataInicio: "2023-06-01", dataFim: null, remuneracao: 95000, estado: "Vigente" },
    { id: 3, colaborador: "Fátima Zacarias Tembe", numero: "CTR-2024/003", tipo: "Contrato a Termo Certo", dataInicio: "2024-03-01", dataFim: "2026-02-28", remuneracao: 65000, estado: "Vigente" },
    { id: 4, colaborador: "José Manuel Sitoe", numero: "CTR-2022/008", tipo: "Contrato por Tempo Indeterminado", dataInicio: "2022-09-01", dataFim: null, remuneracao: 55000, estado: "Vigente" },
    { id: 5, colaborador: "Mariana Inácio Mondlane", numero: "CTR-2025/001", tipo: "Contrato a Termo Certo", dataInicio: "2025-02-01", dataFim: "2026-09-26", remuneracao: 48000, estado: "Vigente" },
    { id: 6, colaborador: "Pedro Henrique Cossa", numero: "CTR-2024/012", tipo: "Contrato por Tempo Indeterminado", dataInicio: "2024-08-15", dataFim: null, remuneracao: 78000, estado: "Vigente" },
    { id: 7, colaborador: "Rosa Amélia Banze", numero: "CTR-2023/022", tipo: "Contrato a Termo Certo", dataInicio: "2023-04-01", dataFim: "2026-09-19", remuneracao: 52000, estado: "Vigente" },
    { id: 8, colaborador: "Tomás Ngwenya Dlamini", numero: "CTR-2024/005", tipo: "Contrato a Termo Certo", dataInicio: "2024-07-01", dataFim: "2026-06-30", remuneracao: 42000, estado: "Vigente" },
    { id: 9, colaborador: "Vanessa Lourenço Matusse", numero: "CTR-2025/008", tipo: "Contrato a Termo Certo", dataInicio: "2025-01-10", dataFim: "2027-01-09", remuneracao: 45000, estado: "Vigente" },
    { id: 10, colaborador: "Wilson António Nhaca", numero: "CTR-2021/003", tipo: "Contrato por Tempo Indeterminado", dataInicio: "2021-11-20", dataFim: null, remuneracao: 88000, estado: "Vigente" },
];

// ---- Mock Data: Candidatos Recrutamento ----
const candidatos = [
    { id: 1, nome: "Alice Tembe", email: "alice@email.com", formacao: "Engenharia Informática", nivel: "Mestrado", estado: "Submetida" },
    { id: 2, nome: "Bruno Cossa", email: "bruno@email.com", formacao: "Gestão", nivel: "Licenciatura", estado: "Em análise" },
    { id: 3, nome: "Clara Machel", email: "clara@email.com", formacao: "Direito", nivel: "Mestrado", estado: "Pré-eliminada" },
    { id: 4, nome: "Daniel Nhantumbo", email: "daniel@email.com", formacao: "Contabilidade", nivel: "Licenciatura", estado: "Seleccionado" },
    { id: 5, nome: "Elsa Guenha", email: "elsa@email.com", formacao: "Recursos Humanos", nivel: "Licenciatura", estado: "Submetida" },
    { id: 6, nome: "Fernando Mabunda", email: "fernando@email.com", formacao: "Engenharia Civil", nivel: "Mestrado", estado: "Em análise" },
    { id: 7, nome: "Gabriela Sitoe", email: "gabriela@email.com", formacao: "Psicologia", nivel: "Licenciatura", estado: "Em análise" },
    { id: 8, nome: "Hugo Mondlane", email: "hugo@email.com", formacao: "Administração Pública", nivel: "Mestrado", estado: "Pré-eliminada" },
    { id: 9, nome: "Irene Banze", email: "irene@email.com", formacao: "Estatística", nivel: "Licenciatura", estado: "Seleccionada" },
    { id: 10, nome: "João Matusse", email: "joao@email.com", formacao: "Gestão Financeira", nivel: "Pós-Graduação", estado: "Submetida" },
    { id: 11, nome: "Keyla Chichava", email: "keyla@email.com", formacao: "Sociologia", nivel: "Mestrado", estado: "Em análise" },
    { id: 12, nome: "Leandro Nhaca", email: "leandro@email.com", formacao: "Informática", nivel: "Licenciatura", estado: "Pré-eliminada" },
];

// ---- Mock Data: Ausências ----
const ausencias = [
    { id: 1, colaborador: "Ana Maria Fernandes", tipo: "Férias", dataInicio: "2026-07-15", dataFim: "2026-07-25", estado: "Aprovada" },
    { id: 2, colaborador: "Carlos Alberto Machava", tipo: "Licença", dataInicio: "2026-08-01", dataFim: "2026-08-15", estado: "Pendente" },
    { id: 3, colaborador: "Fátima Zacarias Tembe", tipo: "Férias", dataInicio: "2026-07-20", dataFim: "2026-07-30", estado: "Aprovada" },
    { id: 4, colaborador: "José Manuel Sitoe", tipo: "Falta", dataInicio: "2026-06-28", dataFim: "2026-06-28", estado: "Aprovada" },
    { id: 5, colaborador: "Mariana Inácio Mondlane", tipo: "Dispensa Médica", dataInicio: "2026-07-01", dataFim: "2026-07-03", estado: "Aprovada" },
    { id: 6, colaborador: "Pedro Henrique Cossa", tipo: "Férias", dataInicio: "2026-08-10", dataFim: "2026-08-20", estado: "Pendente" },
    { id: 7, colaborador: "Rosa Amélia Banze", tipo: "Licença", dataInicio: "2026-09-01", dataFim: "2026-09-30", estado: "Pendente" },
];

// ---- Mock Data: Formação ----
const accoesFormacao = [
    { id: 1, nome: "Formação em Gestão de Projetos", tema: "Gestão", dataInicio: "2026-07-10", dataFim: "2026-07-12", local: "Sala de Conferências A", vagas: 20, inscritos: 15, horas: 12, estado: "Em curso" },
    { id: 2, nome: "Workshop de Liderança", tema: "Liderança", dataInicio: "2026-08-05", dataFim: "2026-08-06", local: "Auditório Principal", vagas: 30, inscritos: 22, horas: 8, estado: "Planeada" },
    { id: 3, nome: "Curso de Informática Básica", tema: "Tecnologia", dataInicio: "2026-06-01", dataFim: "2026-06-15", local: "Laboratório de TI", vagas: 15, inscritos: 15, horas: 20, estado: "Concluída" },
    { id: 4, nome: "Sensibilização em Ética Profissional", tema: "Ética", dataInicio: "2026-09-01", dataFim: "2026-09-01", local: "Sala de Reuniões", vagas: 40, inscritos: 0, horas: 4, estado: "Planeada" },
    { id: 5, nome: "Formação em Segurança no Trabalho", tema: "Segurança", dataInicio: "2026-07-20", dataFim: "2026-07-21", local: "Sala de Conferências B", vagas: 25, inscritos: 18, horas: 8, estado: "Aberta" },
];

// ---- Mock Data: Estágios ----
const estagios = [
    { id: 1, estagiario: "Miguel Santos", instituicao: "Universidade Eduardo Mondlane", tipo: "Académico", unidade: "Departamento de TI", supervisor: "Carlos Alberto Machava", dataInicio: "2026-03-01", dataFim: "2026-08-30", estado: "Em curso" },
    { id: 2, estagiario: "Sara Jone", instituicao: "ISCTEM", tipo: "Académico", unidade: "Secção de Contabilidade", supervisor: "Wilson António Nhaca", dataInicio: "2026-04-15", dataFim: "2026-10-15", estado: "Em curso" },
    { id: 3, estagiario: "Paulo Macuácua", instituicao: "N/A", tipo: "Profissional", unidade: "Divisão de Formação", supervisor: "Rosa Amélia Banze", dataInicio: "2025-09-01", dataFim: "2026-02-28", estado: "Concluído" },
    { id: 4, estagiario: "Adriana Nhantumbo", instituicao: "Universidade Católica de Moçambique", tipo: "Institucional", unidade: "Divisão de RH", supervisor: "Ana Maria Fernandes", dataInicio: "2026-06-01", dataFim: "2026-12-31", estado: "Solicitado" },
];

// ---- Mock Data: Guias de Marcha ----
const guiasMarcha = [
    { id: 1, funcionario: "José Manuel Sitoe", destino: "Maputo", dataPartida: "2026-07-15", dataChegada: "2026-07-18", missao: "Participação em conferência de contabilidade", projecto: "Censo 2027", estado: "Programada" },
    { id: 2, funcionario: "Fátima Zacarias Tembe", destino: "Beira", dataPartida: "2026-07-20", dataChegada: "2026-07-25", missao: "Capacitação de equipas regionais", projecto: "Inquérito Demográfico", estado: "Programada" },
    { id: 3, funcionario: "Pedro Henrique Cossa", destino: "Nampula", dataPartida: "2026-06-10", dataChegada: "2026-06-14", missao: "Supervisão de recolha de dados", projecto: "Censo 2027", estado: "Concluída" },
    { id: 4, funcionario: "Ana Maria Fernandes", destino: "Quelimane", dataPartida: "2026-08-01", dataChegada: "2026-08-05", missao: "Reunião com parceiros locais", projecto: "Inquérito Demográfico", estado: "Programada" },
    { id: 5, funcionario: "Carlos Alberto Machava", destino: "Tete", dataPartida: "2026-06-20", dataChegada: "2026-06-22", missao: "Instalação de equipamento informático", projecto: "Censo 2027", estado: "Em curso" },
];

// ---- Mock Data: Casos de Apoio Social ----
const casosApoio = [
    { id: 1, colaborador: "Tomás Ngwenya Dlamini", tipo: "Doença", descricao: "Licença médica por tratamento", dataRegisto: "2026-06-15", estado: "Em tratamento", canal: "Directo" },
    { id: 2, colaborador: "Anónimo", tipo: "Apoio social", descricao: "Solicitação de apoio financeiro por emergência familiar", dataRegisto: "2026-06-20", estado: "Registado", canal: "Canal confidencial" },
    { id: 3, colaborador: "Vanessa Lourenço Matusse", tipo: "Emergência", descricao: "Acolhimento temporário - situação de emergência habitacional", dataRegisto: "2026-06-18", estado: "Encaminhado", canal: "Directo" },
    { id: 4, colaborador: "Anónimo", tipo: "Doença", descricao: "Relato de condição de saúde mental - pedido de sigilo", dataRegisto: "2026-06-22", estado: "Registado", canal: "Canal confidencial" },
];

// ---- Mock Data: Inquéritos ----
const inqueritos = [
    { id: 1, titulo: "Inquérito de Clima Organizacional 2026", estado: "Concluído", respostas: 142, total: 150, dataInicio: "2026-05-01", dataFim: "2026-05-31" },
    { id: 2, titulo: "Satisfação com Infraestrutura", estado: "Em curso", respostas: 89, total: 150, dataInicio: "2026-06-15", dataFim: "2026-07-15" },
    { id: 3, titulo: "Avaliação de Formação - Q2 2026", estado: "Publicado", respostas: 0, total: 35, dataInicio: "2026-07-01", dataFim: "2026-07-31" },
];

// ---- Funções Utilitárias ----
function calcularIdade(dataNascimento) {
    const hoje = new Date();
    const nasc = new Date(dataNascimento);
    let idade = hoje.getFullYear() - nasc.getFullYear();
    const m = hoje.getMonth() - nasc.getMonth();
    if (m < 0 || (m === 0 && hoje.getDate() < nasc.getDate())) idade--;
    return idade;
}

function calcularTempoServico(dataIngresso) {
    const hoje = new Date();
    const ingresso = new Date(dataIngresso);
    let anos = hoje.getFullYear() - ingresso.getFullYear();
    const m = hoje.getMonth() - ingresso.getMonth();
    if (m < 0 || (m === 0 && hoje.getDate() < ingresso.getDate())) anos--;
    return anos;
}

function formatarData(data) {
    if (!data) return "—";
    const d = new Date(data);
    return d.toLocaleDateString('pt-MZ', { day: '2-digit', month: '2-digit', year: 'numeric' });
}

function formatarMoeda(valor) {
    return new Intl.NumberFormat('pt-MZ', { style: 'currency', currency: 'MZN' }).format(valor);
}

function diasRestantes(dataFim) {
    if (!dataFim) return Infinity;
    const hoje = new Date();
    const fim = new Date(dataFim);
    return Math.ceil((fim - hoje) / (1000 * 60 * 60 * 24));
}

function getBadgeEstado(estado) {
    const map = {
        'Ativo': 'badge-success', 'Inativo': 'badge-gray', 'Suspenso': 'badge-warning',
        'Vigente': 'badge-success', 'Expirado': 'badge-danger', 'Rescindido': 'badge-danger',
        'Submetida': 'badge-info', 'Em análise': 'badge-info', 'Pré-eliminada': 'badge-danger',
        'Seleccionado': 'badge-success', 'Seleccionada': 'badge-success', 'Não selecionada': 'badge-gray',
        'Aprovada': 'badge-success', 'Pendente': 'badge-warning', 'Rejeitada': 'badge-danger',
        'Em curso': 'badge-info', 'Concluída': 'badge-success', 'Concluído': 'badge-success',
        'Planeada': 'badge-gray', 'Aberta': 'badge-info', 'Cancelada': 'badge-danger',
        'Programada': 'badge-info', 'Em tratamento': 'badge-warning', 'Registado': 'badge-gray',
        'Encaminhado': 'badge-info', 'Resolvido': 'badge-success',
        'Rascunho': 'badge-gray', 'Aprovado': 'badge-success', 'Em execução': 'badge-info',
        'Publicado': 'badge-purple', 'Solicitado': 'badge-warning',
        'Enviada': 'badge-success', 'Lida': 'badge-info', 'Erro': 'badge-danger',
    };
    return map[estado] || 'badge-gray';
}

// ---- Toast Notifications (jQuery) ----
function showToast(mensagem, tipo) {
    tipo = tipo || 'success';
    var icons = { success: '✓', warning: '⚠', danger: '✕' };
    var $toast = $('<div class="toast toast-' + tipo + '">' +
        '<span class="toast-icon">' + (icons[tipo] || '✓') + '</span>' +
        '<span class="toast-message">' + mensagem + '</span>' +
        '</div>');
    $('#toast-container').append($toast);
    setTimeout(function() {
        $toast.css('opacity', '0');
        setTimeout(function() { $toast.remove(); }, 300);
    }, 3000);
}

// ---- Sidebar Toggle (jQuery) ----
$(document).ready(function() {
    $('.topbar-toggle').on('click', function() {
        $('.sidebar').toggleClass('open');
        $('.sidebar-overlay').toggleClass('active');
    });
    $('.sidebar-overlay').on('click', function() {
        $('.sidebar').removeClass('open');
        $(this).removeClass('active');
    });
});

// ---- Modal Helpers (jQuery) ----
function openModal(id) {
    $('#' + id).addClass('active');
}

function closeModal(id) {
    $('#' + id).removeClass('active');
}

$(document).ready(function() {
    $('.modal-overlay').on('click', function(e) {
        if ($(e.target).is('.modal-overlay')) {
            $(this).removeClass('active');
        }
    });
});

// ---- Alerta Helper (jQuery) ----
function mostrarAlerta(titulo, corpo, footer) {
    $('#alertaTitulo').text(titulo);
    $('#alertaCorpo').html(corpo);
    $('#alertaFooter').html(footer);
    openModal('modalAlerta');
}
