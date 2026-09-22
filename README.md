# SGRH — Sistema de Gestão de Recursos Humanos (INE)

Aplicação ASP.NET Core MVC (net9.0) com Entity Framework Core e SQL Server.

## Configuração do ambiente (obrigatório)

A connection string **não** deve ser editada no `appsettings.json` (esse ficheiro é partilhado via git e muda entre máquinas/ramos). Em vez disso, cria um ficheiro local:

```
SGRH/appsettings.Development.json
```

com o seguinte conteúdo (ajusta o servidor à tua instância):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SGRH;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Este ficheiro está no `.gitignore`, por isso:
- nunca é enviado para o repositório;
- tem **prioridade** sobre o `appsettings.json` quando a app corre em ambiente Development (o padrão no `launchSettings.json`);
- cada programador mantém a sua ligação sem conflitos no `git pull`.

> Exemplos de connection strings comuns:
> - SQL Server local (Windows Auth): `Server=localhost;Database=SGRH;Trusted_Connection=True;TrustServerCertificate=True;`
> - LocalDB: `Server=(localdb)\MSSQLLocalDB;Database=SGRH;Trusted_Connection=True;TrustServerCertificate=True;`
> - SQL Auth: `Server=localhost;Database=SGRH;User Id=sa;Password=<senha>;TrustServerCertificate=True;`

## Base de dados

Aplicar as migrations (criam o schema e os dados seed):

```bash
cd SGRH
dotnet ef database update
```

## Correr

```bash
cd SGRH
dotnet run
```

A app arranca em `http://localhost:5267` (perfil http).

## Credenciais iniciais

O seed do `Program.cs` garante a existência do utilizador `admin` com perfil Administrador. Se o login falhar após um pull que altere o seed, confirma na tabela `utilizador_sistema` (a hash pode ter sido substituída) ou pede ao administrador para redefinir a senha no módulo Administração → Utilizadores.
