using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SGRH.Data;
using SGRH.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// ── Serviços da Administração do Sistema ───────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SGRH.Services.IPasswordHasher, SGRH.Services.PasswordHasher>();
builder.Services.AddScoped<SGRH.Services.IAuditoriaService, SGRH.Services.AuditoriaService>();

// ── Base de dados ──────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Autorização global: por defeito, todas as páginas exigem utilizador autenticado
// (o login e o registo continuam públicos via [AllowAnonymous])
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
});

builder.Services.AddScoped<AutorizacaoService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Ficheiros estáticos servidos antes da autorização — sem isto, a FallbackPolicy
// redireciona o CSS/JS (ex.: /css/styles.css) para o login e a página quebra.
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

// Fallback: URLs só com o controller (ex.: /Admin → Admin/Index)
app.MapControllerRoute(
    name: "controllerIndex",
    pattern: "{controller}/{action=Index}/{id?}")
    .WithStaticAssets();

// ── Seed de perfis e permissões ────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.ExecuteSqlRaw(@"
        -- ═══════════════════════════════════════════════════════════
        -- PERFIS DE ACESSO (13 perfis)
        -- ═══════════════════════════════════════════════════════════
        SET IDENTITY_INSERT perfil_acesso ON;

        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 1)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (1, 'Administrador', 'Acesso total ao sistema', 3);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 2)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (2, 'RH', 'Gestao de recursos humanos', 2);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 3)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (3, 'Gestor', 'Gestao de equipas e processos', 2);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 4)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (4, 'Direccao', 'Direcao estrategica da organizacao', 3);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 5)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (5, 'Funcionario', 'Funcionario ou formador de apoio', 1);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 6)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (6, 'Juri', 'Membro de jury de recrutamento', 2);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 7)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (7, 'Juridico', 'Assessoria juridica e contratos', 2);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 8)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (8, 'Formador', 'Formador externo ou interno', 1);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 9)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (9, 'Supervisor', 'Supervisor de estagios', 2);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 10)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (10, 'Estagiario', 'Estagiario da organizacao', 1);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 11)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (11, 'Candidato', 'Candidato a recrutamento', 1);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 12)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (12, 'InstituicaoEnsino', 'Instituicao de ensino parceira', 1);
        IF NOT EXISTS (SELECT 1 FROM perfil_acesso WHERE id_perfil = 13)
            INSERT INTO perfil_acesso (id_perfil, nome, descricao, nivel_confidencialidade)
            VALUES (13, 'TI', 'Suporte tecnico e administracao', 3);

        SET IDENTITY_INSERT perfil_acesso OFF;

        -- Tipos de documento (FK obrigatoria em documento; sem este seed, todo o upload
        -- de documento falha com DbUpdateException por violacao de FK)
        SET IDENTITY_INSERT tipo_documento ON;

        IF NOT EXISTS (SELECT 1 FROM tipo_documento WHERE id_tipo_documento = 1)
            INSERT INTO tipo_documento (id_tipo_documento, nome)
            VALUES (1, 'Documento de Identificação');
        IF NOT EXISTS (SELECT 1 FROM tipo_documento WHERE id_tipo_documento = 2)
            INSERT INTO tipo_documento (id_tipo_documento, nome)
            VALUES (2, 'Contrato');
        IF NOT EXISTS (SELECT 1 FROM tipo_documento WHERE id_tipo_documento = 3)
            INSERT INTO tipo_documento (id_tipo_documento, nome)
            VALUES (3, 'Certificado');
        IF NOT EXISTS (SELECT 1 FROM tipo_documento WHERE id_tipo_documento = 4)
            INSERT INTO tipo_documento (id_tipo_documento, nome)
            VALUES (4, 'Curriculum Vitae');
        IF NOT EXISTS (SELECT 1 FROM tipo_documento WHERE id_tipo_documento = 5)
            INSERT INTO tipo_documento (id_tipo_documento, nome)
            VALUES (5, 'Outro');

        SET IDENTITY_INSERT tipo_documento OFF;

        -- Tipos de ausência (dropdown ""Registar Ausência"" fica vazio sem este seed)
        SET IDENTITY_INSERT tipo_ausencia ON;

        IF NOT EXISTS (SELECT 1 FROM tipo_ausencia WHERE id_tipo_ausencia = 1)
            INSERT INTO tipo_ausencia (id_tipo_ausencia, nome, descricao, dias_maximos)
            VALUES (1, 'Férias', 'Férias anuais do colaborador', 22);
        IF NOT EXISTS (SELECT 1 FROM tipo_ausencia WHERE id_tipo_ausencia = 2)
            INSERT INTO tipo_ausencia (id_tipo_ausencia, nome, descricao, dias_maximos)
            VALUES (2, 'Falta Justificada', 'Falta com justificação aceite', 5);
        IF NOT EXISTS (SELECT 1 FROM tipo_ausencia WHERE id_tipo_ausencia = 3)
            INSERT INTO tipo_ausencia (id_tipo_ausencia, nome, descricao, dias_maximos)
            VALUES (3, 'Falta Injustificada', 'Falta sem justificação', NULL);
        IF NOT EXISTS (SELECT 1 FROM tipo_ausencia WHERE id_tipo_ausencia = 4)
            INSERT INTO tipo_ausencia (id_tipo_ausencia, nome, descricao, dias_maximos)
            VALUES (4, 'Licença Médica', 'Ausência por motivo de saúde', 30);
        IF NOT EXISTS (SELECT 1 FROM tipo_ausencia WHERE id_tipo_ausencia = 5)
            INSERT INTO tipo_ausencia (id_tipo_ausencia, nome, descricao, dias_maximos)
            VALUES (5, 'Licença de Maternidade/Paternidade', 'Licença por nascimento ou adopção', 60);
        IF NOT EXISTS (SELECT 1 FROM tipo_ausencia WHERE id_tipo_ausencia = 6)
            INSERT INTO tipo_ausencia (id_tipo_ausencia, nome, descricao, dias_maximos)
            VALUES (6, 'Licença Sem Vencimento', 'Ausência autorizada sem remuneração', 90);
        IF NOT EXISTS (SELECT 1 FROM tipo_ausencia WHERE id_tipo_ausencia = 7)
            INSERT INTO tipo_ausencia (id_tipo_ausencia, nome, descricao, dias_maximos)
            VALUES (7, 'Luto', 'Falecimento de familiar próximo', 5);
        IF NOT EXISTS (SELECT 1 FROM tipo_ausencia WHERE id_tipo_ausencia = 8)
            INSERT INTO tipo_ausencia (id_tipo_ausencia, nome, descricao, dias_maximos)
            VALUES (8, 'Outro', 'Outro motivo de ausência', NULL);

        SET IDENTITY_INSERT tipo_ausencia OFF;

        -- Tipos de pedido (dropdown ""Novo Pedido"" fica vazio sem este seed)
        SET IDENTITY_INSERT tipo_pedido ON;

        IF NOT EXISTS (SELECT 1 FROM tipo_pedido WHERE id_tipo_pedido = 1)
            INSERT INTO tipo_pedido (id_tipo_pedido, nome)
            VALUES (1, 'Pedido de Férias');
        IF NOT EXISTS (SELECT 1 FROM tipo_pedido WHERE id_tipo_pedido = 2)
            INSERT INTO tipo_pedido (id_tipo_pedido, nome)
            VALUES (2, 'Justificação de Falta');
        IF NOT EXISTS (SELECT 1 FROM tipo_pedido WHERE id_tipo_pedido = 3)
            INSERT INTO tipo_pedido (id_tipo_pedido, nome)
            VALUES (3, 'Alteração de Dados Pessoais');
        IF NOT EXISTS (SELECT 1 FROM tipo_pedido WHERE id_tipo_pedido = 4)
            INSERT INTO tipo_pedido (id_tipo_pedido, nome)
            VALUES (4, 'Requerimento Geral');
        IF NOT EXISTS (SELECT 1 FROM tipo_pedido WHERE id_tipo_pedido = 5)
            INSERT INTO tipo_pedido (id_tipo_pedido, nome)
            VALUES (5, 'Outro');

        SET IDENTITY_INSERT tipo_pedido OFF;

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - ADMINISTRADOR (Perfil 1) - Total em tudo
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 1)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (1, 'Dashboard', 1, 1, 1, 1),
                (1, 'Colaboradores', 1, 1, 1, 1),
                (1, 'Contratos', 1, 1, 1, 1),
                (1, 'Administracao', 1, 1, 1, 1),
                (1, 'Formacao', 1, 1, 1, 1),
                (1, 'Estagios', 1, 1, 1, 1),
                (1, 'GuiasMarcha', 1, 1, 1, 1),
                (1, 'Recrutamento', 1, 1, 1, 1),
                (1, 'Clima', 1, 1, 1, 1),
                (1, 'AssuntosSociais', 1, 1, 1, 1),
                (1, 'Reporting', 1, 1, 1, 1),
                (1, 'Utilizadores', 1, 1, 1, 1),
                (1, 'Auditoria', 1, 1, 1, 1),
                (1, 'Configuracoes', 1, 1, 1, 1);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - RH (Perfil 2)
        -- Cadastro=Total, Recrutamento=Total, Contratos=Total,
        -- Admin=Total, Formacao=Total, Estagios=Total,
        -- Clima=Restrito, AssuntosSociais=Elevado,
        -- Reporting=Elevado, AdminSys=Total
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 2)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (2, 'Dashboard', 1, 1, 1, 1),
                (2, 'Colaboradores', 1, 1, 1, 1),
                (2, 'Contratos', 1, 1, 1, 1),
                (2, 'Administracao', 1, 1, 1, 1),
                (2, 'Formacao', 1, 1, 1, 1),
                (2, 'Estagios', 1, 1, 1, 1),
                (2, 'GuiasMarcha', 1, 1, 1, 1),
                (2, 'Recrutamento', 1, 1, 1, 1),
                (2, 'Clima', 1, 0, 0, 0),
                (2, 'AssuntosSociais', 1, 1, 1, 0),
                (2, 'Reporting', 1, 1, 1, 0),
                (2, 'Utilizadores', 1, 1, 1, 1),
                (2, 'Auditoria', 1, 1, 1, 1),
                (2, 'Configuracoes', 1, 1, 1, 1);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - GESTOR (Perfil 3)
        -- Cadastro=Consulta, Recrutamento=Consulta, Contratos=Consulta,
        -- Admin=Aprovacao, Formacao=Consulta, Estagios=SeSupervisor,
        -- Clima=Consulta, AssuntosSociais=Nao, Reporting=Consulta,
        -- AdminSys=Nao
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 3)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (3, 'Dashboard', 1, 0, 0, 0),
                (3, 'Colaboradores', 1, 0, 0, 0),
                (3, 'Contratos', 1, 0, 0, 0),
                (3, 'Administracao', 1, 0, 1, 0),
                (3, 'Formacao', 1, 0, 0, 0),
                (3, 'Estagios', 1, 0, 0, 0),
                (3, 'GuiasMarcha', 1, 0, 1, 0),
                (3, 'Recrutamento', 1, 0, 0, 0),
                (3, 'Clima', 1, 0, 0, 0),
                (3, 'AssuntosSociais', 0, 0, 0, 0),
                (3, 'Reporting', 1, 0, 0, 0),
                (3, 'Utilizadores', 0, 0, 0, 0),
                (3, 'Auditoria', 0, 0, 0, 0),
                (3, 'Configuracoes', 0, 0, 0, 0);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - DIRECAO (Perfil 4)
        -- Cadastro=Consulta, Recrutamento=Consulta, Contratos=Consulta,
        -- Admin=Consulta, Formacao=Consulta, Estagios=Consulta,
        -- Clima=Consulta, AssuntosSociais=ConsultaAgregada,
        -- Reporting=Elevado, AdminSys=Nao
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 4)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (4, 'Dashboard', 1, 0, 0, 0),
                (4, 'Colaboradores', 1, 0, 0, 0),
                (4, 'Contratos', 1, 0, 0, 0),
                (4, 'Administracao', 1, 0, 0, 0),
                (4, 'Formacao', 1, 0, 0, 0),
                (4, 'Estagios', 1, 0, 0, 0),
                (4, 'GuiasMarcha', 1, 0, 0, 0),
                (4, 'Recrutamento', 1, 0, 0, 0),
                (4, 'Clima', 1, 0, 0, 0),
                (4, 'AssuntosSociais', 1, 0, 0, 0),
                (4, 'Reporting', 1, 1, 1, 0),
                (4, 'Utilizadores', 0, 0, 0, 0),
                (4, 'Auditoria', 0, 0, 0, 0),
                (4, 'Configuracoes', 0, 0, 0, 0);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - FUNCIONARIO/FAE (Perfil 5)
        -- Cadastro=Proprio, Recrutamento=Consulta, Contratos=Proprio,
        -- Admin=Solicitacao, Formacao=Proprio, Estagios=Proprio,
        -- Clima=Participacao, AssuntosSociais=Proprio,
        -- Reporting=PropriosDados, AdminSys=Nao
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 5)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (5, 'Dashboard', 1, 0, 0, 0),
                (5, 'Colaboradores', 1, 0, 0, 0),
                (5, 'Contratos', 1, 0, 0, 0),
                (5, 'Administracao', 1, 1, 0, 0),
                (5, 'Formacao', 1, 0, 0, 0),
                (5, 'Estagios', 1, 0, 0, 0),
                (5, 'GuiasMarcha', 1, 1, 0, 0),
                (5, 'Recrutamento', 1, 0, 0, 0),
                (5, 'Clima', 1, 1, 0, 0),
                (5, 'AssuntosSociais', 1, 0, 0, 0),
                (5, 'Reporting', 1, 0, 0, 0),
                (5, 'Utilizadores', 0, 0, 0, 0),
                (5, 'Auditoria', 0, 0, 0, 0),
                (5, 'Configuracoes', 0, 0, 0, 0);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - JURI (Perfil 6)
        -- Cadastro=Nao, Recrutamento=Avaliacao, Resto=Nao,
        -- Reporting=Recrutamento
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 6)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (6, 'Dashboard', 1, 0, 0, 0),
                (6, 'Colaboradores', 0, 0, 0, 0),
                (6, 'Contratos', 0, 0, 0, 0),
                (6, 'Administracao', 0, 0, 0, 0),
                (6, 'Formacao', 0, 0, 0, 0),
                (6, 'Estagios', 0, 0, 0, 0),
                (6, 'GuiasMarcha', 0, 0, 0, 0),
                (6, 'Recrutamento', 1, 1, 0, 0),
                (6, 'Clima', 0, 0, 0, 0),
                (6, 'AssuntosSociais', 0, 0, 0, 0),
                (6, 'Reporting', 1, 0, 0, 0),
                (6, 'Utilizadores', 0, 0, 0, 0),
                (6, 'Auditoria', 0, 0, 0, 0),
                (6, 'Configuracoes', 0, 0, 0, 0);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - JURIDICO (Perfil 7)
        -- Cadastro=ConsultaNecessaria, Contratos=Elevado,
        -- Reporting=Contratos, Resto=Nao
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 7)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (7, 'Dashboard', 1, 0, 0, 0),
                (7, 'Colaboradores', 1, 0, 0, 0),
                (7, 'Contratos', 1, 1, 1, 0),
                (7, 'Administracao', 0, 0, 0, 0),
                (7, 'Formacao', 0, 0, 0, 0),
                (7, 'Estagios', 0, 0, 0, 0),
                (7, 'GuiasMarcha', 0, 0, 0, 0),
                (7, 'Recrutamento', 0, 0, 0, 0),
                (7, 'Clima', 0, 0, 0, 0),
                (7, 'AssuntosSociais', 0, 0, 0, 0),
                (7, 'Reporting', 1, 0, 0, 0),
                (7, 'Utilizadores', 0, 0, 0, 0),
                (7, 'Auditoria', 0, 0, 0, 0),
                (7, 'Configuracoes', 0, 0, 0, 0);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - FORMADOR (Perfil 8)
        -- Cadastro=ConsultaNecessaria, Formacao=Elevado,
        -- Reporting=Formacao, Resto=Nao
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 8)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (8, 'Dashboard', 1, 0, 0, 0),
                (8, 'Colaboradores', 1, 0, 0, 0),
                (8, 'Contratos', 0, 0, 0, 0),
                (8, 'Administracao', 0, 0, 0, 0),
                (8, 'Formacao', 1, 1, 1, 0),
                (8, 'Estagios', 0, 0, 0, 0),
                (8, 'GuiasMarcha', 0, 0, 0, 0),
                (8, 'Recrutamento', 0, 0, 0, 0),
                (8, 'Clima', 0, 0, 0, 0),
                (8, 'AssuntosSociais', 0, 0, 0, 0),
                (8, 'Reporting', 1, 0, 0, 0),
                (8, 'Utilizadores', 0, 0, 0, 0),
                (8, 'Auditoria', 0, 0, 0, 0),
                (8, 'Configuracoes', 0, 0, 0, 0);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - SUPERVISOR (Perfil 9)
        -- Cadastro=ConsultaNecessaria, Estagios=Elevado,
        -- Reporting=Estagios, Resto=Nao
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 9)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (9, 'Dashboard', 1, 0, 0, 0),
                (9, 'Colaboradores', 1, 0, 0, 0),
                (9, 'Contratos', 0, 0, 0, 0),
                (9, 'Administracao', 0, 0, 0, 0),
                (9, 'Formacao', 0, 0, 0, 0),
                (9, 'Estagios', 1, 1, 1, 0),
                (9, 'GuiasMarcha', 0, 0, 0, 0),
                (9, 'Recrutamento', 0, 0, 0, 0),
                (9, 'Clima', 0, 0, 0, 0),
                (9, 'AssuntosSociais', 0, 0, 0, 0),
                (9, 'Reporting', 1, 0, 0, 0),
                (9, 'Utilizadores', 0, 0, 0, 0),
                (9, 'Auditoria', 0, 0, 0, 0),
                (9, 'Configuracoes', 0, 0, 0, 0);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - ESTAGIARIO (Perfil 10)
        -- Cadastro=Proprio, Estagios=Limitado,
        -- Reporting=PropriosDados, Resto=Nao
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 10)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (10, 'Dashboard', 1, 0, 0, 0),
                (10, 'Colaboradores', 1, 0, 0, 0),
                (10, 'Contratos', 0, 0, 0, 0),
                (10, 'Administracao', 0, 0, 0, 0),
                (10, 'Formacao', 0, 0, 0, 0),
                (10, 'Estagios', 1, 1, 0, 0),
                (10, 'GuiasMarcha', 0, 0, 0, 0),
                (10, 'Recrutamento', 0, 0, 0, 0),
                (10, 'Clima', 0, 0, 0, 0),
                (10, 'AssuntosSociais', 0, 0, 0, 0),
                (10, 'Reporting', 1, 0, 0, 0),
                (10, 'Utilizadores', 0, 0, 0, 0),
                (10, 'Auditoria', 0, 0, 0, 0),
                (10, 'Configuracoes', 0, 0, 0, 0);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - CANDIDATO (Perfil 11)
        -- Recrutamento=Proprio, Resto=Nao
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 11)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (11, 'Dashboard', 1, 0, 0, 0),
                (11, 'Colaboradores', 0, 0, 0, 0),
                (11, 'Contratos', 0, 0, 0, 0),
                (11, 'Administracao', 0, 0, 0, 0),
                (11, 'Formacao', 0, 0, 0, 0),
                (11, 'Estagios', 0, 0, 0, 0),
                (11, 'GuiasMarcha', 0, 0, 0, 0),
                (11, 'Recrutamento', 1, 1, 0, 0),
                (11, 'Clima', 0, 0, 0, 0),
                (11, 'AssuntosSociais', 0, 0, 0, 0),
                (11, 'Reporting', 0, 0, 0, 0),
                (11, 'Utilizadores', 0, 0, 0, 0),
                (11, 'Auditoria', 0, 0, 0, 0),
                (11, 'Configuracoes', 0, 0, 0, 0);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - INSTITUICAO DE ENSINO (Perfil 12)
        -- Estagios=Limitado, Reporting=Estagios, Resto=Nao
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 12)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (12, 'Dashboard', 1, 0, 0, 0),
                (12, 'Colaboradores', 0, 0, 0, 0),
                (12, 'Contratos', 0, 0, 0, 0),
                (12, 'Administracao', 0, 0, 0, 0),
                (12, 'Formacao', 0, 0, 0, 0),
                (12, 'Estagios', 1, 1, 0, 0),
                (12, 'GuiasMarcha', 0, 0, 0, 0),
                (12, 'Recrutamento', 0, 0, 0, 0),
                (12, 'Clima', 0, 0, 0, 0),
                (12, 'AssuntosSociais', 0, 0, 0, 0),
                (12, 'Reporting', 1, 0, 0, 0),
                (12, 'Utilizadores', 0, 0, 0, 0),
                (12, 'Auditoria', 0, 0, 0, 0),
                (12, 'Configuracoes', 0, 0, 0, 0);
        END

        -- ═══════════════════════════════════════════════════════════
        -- PERMISSOES - TI (Perfil 13)
        -- Total tecnico em todos os modulos
        -- ═══════════════════════════════════════════════════════════
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 13)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (13, 'Dashboard', 1, 1, 1, 1),
                (13, 'Colaboradores', 1, 1, 1, 1),
                (13, 'Contratos', 1, 1, 1, 1),
                (13, 'Administracao', 1, 1, 1, 1),
                (13, 'Formacao', 1, 1, 1, 1),
                (13, 'Estagios', 1, 1, 1, 1),
                (13, 'GuiasMarcha', 1, 1, 1, 1),
                (13, 'Recrutamento', 1, 1, 1, 1),
                (13, 'Clima', 1, 1, 1, 1),
                (13, 'AssuntosSociais', 1, 1, 1, 1),
                (13, 'Reporting', 1, 1, 1, 1),
                (13, 'Utilizadores', 1, 1, 1, 1),
                (13, 'Auditoria', 1, 1, 1, 1),
                (13, 'Configuracoes', 1, 1, 1, 1);
        END

        -- Utilizador admin
        IF NOT EXISTS (SELECT 1 FROM utilizador_sistema WHERE username = 'admin')
            INSERT INTO utilizador_sistema (username, password_hash, email, id_perfil, estado, data_criacao)
            VALUES ('admin', '$2b$12$uPpCL5PrcrMtPxiW3oflXutIQg6aC4lcOzKuXKHF29rOaN57fvnfy', 'admin@ine.gov.mz', 1, 'Ativo', '2025-01-01');
        ELSE
            UPDATE utilizador_sistema SET password_hash = '$2b$12$uPpCL5PrcrMtPxiW3oflXutIQg6aC4lcOzKuXKHF29rOaN57fvnfy' WHERE username = 'admin';
    ");
}

app.Run();
