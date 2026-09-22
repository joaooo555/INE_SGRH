using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SGRH.Data;
using SGRH.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

var app = builder.Build();

// Configure the HTTP request pipeline.
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

// ── Seed de dados de acesso ────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.ExecuteSqlRaw(@"
        -- Perfis de acesso (ID = explícito, precisa de IDENTITY_INSERT ON)
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
            VALUES (4, 'FAE', 'Formacao e apoio social', 1);

        SET IDENTITY_INSERT perfil_acesso OFF;

        -- Permissoes Administrador (Perfil 1)
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

        -- Permissoes RH (Perfil 2)
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 2)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (2, 'Dashboard', 1, 1, 1, 1),
                (2, 'Colaboradores', 1, 1, 1, 1),
                (2, 'Contratos', 1, 1, 1, 1),
                (2, 'Formacao', 1, 1, 1, 1),
                (2, 'Estagios', 1, 1, 1, 1),
                (2, 'GuiasMarcha', 1, 1, 1, 1),
                (2, 'Recrutamento', 1, 1, 1, 1),
                (2, 'Clima', 1, 1, 1, 1),
                (2, 'AssuntosSociais', 1, 1, 1, 1),
                (2, 'Reporting', 1, 1, 1, 1);
        END

        -- Permissoes Gestor (Perfil 3) - consulta em todos os modulos
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 3)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (3, 'Dashboard', 1, 0, 0, 0),
                (3, 'Colaboradores', 1, 0, 0, 0),
                (3, 'Contratos', 1, 0, 0, 0),
                (3, 'Administracao', 1, 0, 0, 0),
                (3, 'Formacao', 1, 0, 0, 0),
                (3, 'Estagios', 1, 0, 0, 0),
                (3, 'GuiasMarcha', 1, 0, 0, 0),
                (3, 'Recrutamento', 1, 0, 0, 0),
                (3, 'Clima', 1, 0, 0, 0),
                (3, 'AssuntosSociais', 1, 0, 0, 0),
                (3, 'Reporting', 1, 0, 0, 0),
                (3, 'Utilizadores', 1, 0, 0, 0),
                (3, 'Auditoria', 1, 0, 0, 0),
                (3, 'Configuracoes', 1, 0, 0, 0);
        END

        -- Permissoes FAE (Perfil 4) - consulta em Dashboard, Formacao e AssuntosSociais
        IF NOT EXISTS (SELECT 1 FROM permissao WHERE id_perfil = 4)
        BEGIN
            INSERT INTO permissao (id_perfil, modulo, pode_visualizar, pode_criar, pode_editar, pode_eliminar)
            VALUES
                (4, 'Dashboard', 1, 0, 0, 0),
                (4, 'Formacao', 1, 0, 0, 0),
                (4, 'AssuntosSociais', 1, 0, 0, 0);
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
