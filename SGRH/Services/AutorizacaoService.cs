using Microsoft.EntityFrameworkCore;
using SGRH.Data;
using SGRH.Models;

namespace SGRH.Services;

public class AutorizacaoService
{
    private readonly AppDbContext _db;
    private readonly Dictionary<int, Dictionary<string, Permissao>> _cache = new();
    private readonly object _lock = new();

    public AutorizacaoService(AppDbContext db)
    {
        _db = db;
    }

    private async Task<Dictionary<string, Permissao>> ObterPermissoesPerfil(int idPerfil)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(idPerfil, out var cached))
                return cached;
        }

        var permissoes = await _db.Permissoes
            .Where(p => p.IdPerfil == idPerfil)
            .ToListAsync();

        var dict = permissoes.ToDictionary(p => p.Modulo, p => p);

        lock (_lock)
        {
            _cache[idPerfil] = dict;
        }

        return dict;
    }

    public async Task<bool> PodeVisualizar(int idPerfil, string modulo)
    {
        var permissoes = await ObterPermissoesPerfil(idPerfil);
        return permissoes.TryGetValue(modulo, out var p) && p.PodeVisualizar;
    }

    public async Task<bool> PodeCriar(int idPerfil, string modulo)
    {
        var permissoes = await ObterPermissoesPerfil(idPerfil);
        return permissoes.TryGetValue(modulo, out var p) && p.PodeCriar;
    }

    public async Task<bool> PodeEditar(int idPerfil, string modulo)
    {
        var permissoes = await ObterPermissoesPerfil(idPerfil);
        return permissoes.TryGetValue(modulo, out var p) && p.PodeEditar;
    }

    public async Task<bool> PodeEliminar(int idPerfil, string modulo)
    {
        var permissoes = await ObterPermissoesPerfil(idPerfil);
        return permissoes.TryGetValue(modulo, out var p) && p.PodeEliminar;
    }

    public async Task<Permissao?> ObterPermissao(int idPerfil, string modulo)
    {
        var permissoes = await ObterPermissoesPerfil(idPerfil);
        permissoes.TryGetValue(modulo, out var p);
        return p;
    }

    public async Task<Dictionary<string, Permissao>> ObterTodasPermissoes(int idPerfil)
    {
        return await ObterPermissoesPerfil(idPerfil);
    }

    public void LimparCache(int? idPerfil = null)
    {
        lock (_lock)
        {
            if (idPerfil.HasValue)
                _cache.Remove(idPerfil.Value);
            else
                _cache.Clear();
        }
    }
}
