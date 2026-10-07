using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SGRH.Authorization;
using SGRH.Data;
using SGRH.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<PermissoesViewBagFilter>();
});

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

// Fallback: URLs só com o controller (ex.: /Home → Home/Index)
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

        -- ═══════════════════════════════════════════════════════════
        -- RECONCILIACAO DOS NOMES DOS PERFIS
        -- Bases criadas por versoes anteriores do prototipo usam nomes
        -- diferentes para os mesmos ids (2='Gestor RH', 3='Director',
        -- 4='Utilizador'). O seed so insere quando a linha falta, por isso
        -- aqui se forcam os nomes/descricoes da matriz funcional. E' apenas
        -- rotulo: as chaves estrangeiras (utilizador_sistema.id_perfil)
        -- continuam a apontar para o mesmo perfil.
        -- ═══════════════════════════════════════════════════════════
        UPDATE perfil_acesso SET nome = v.nome, descricao = v.descricao
        FROM (VALUES
            (1,  'Administrador',      'Acesso total ao sistema'),
            (2,  'RH',                 'Gestao de recursos humanos'),
            (3,  'Gestor',             'Gestao de equipas e processos'),
            (4,  'Direccao',           'Direcao estrategica da organizacao'),
            (5,  'Funcionario',        'Funcionario ou formador de apoio'),
            (6,  'Juri',               'Membro de jury de recrutamento'),
            (7,  'Juridico',           'Assessoria juridica e contratos'),
            (8,  'Formador',           'Formador externo ou interno'),
            (9,  'Supervisor',         'Supervisor de estagios'),
            (10, 'Estagiario',         'Estagiario da organizacao'),
            (11, 'Candidato',          'Candidato a recrutamento'),
            (12, 'InstituicaoEnsino',  'Instituicao de ensino parceira'),
            (13, 'TI',                 'Suporte tecnico e administracao')
        ) AS v(id_perfil, nome, descricao)
        WHERE perfil_acesso.id_perfil = v.id_perfil;

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

        -- Utilizador admin
        IF NOT EXISTS (SELECT 1 FROM utilizador_sistema WHERE username = 'admin')
            INSERT INTO utilizador_sistema (username, password_hash, email, id_perfil, estado, data_criacao)
            VALUES ('admin', '$2b$12$uPpCL5PrcrMtPxiW3oflXutIQg6aC4lcOzKuXKHF29rOaN57fvnfy', 'admin@ine.gov.mz', 1, 'Ativo', '2025-01-01');
        ELSE
            UPDATE utilizador_sistema SET password_hash = '$2b$12$uPpCL5PrcrMtPxiW3oflXutIQg6aC4lcOzKuXKHF29rOaN57fvnfy' WHERE username = 'admin';
    ");

    // A matriz de acessos é a fonte de verdade das permissões. Reescreve a
    // tabela permissao com os níveis funcionais e deriva os quatro booleanos
    // a partir deles, para que a autorização existente continue intacta.
    await MatrizAcesso.AplicarAsync(db);
}

app.Run();
