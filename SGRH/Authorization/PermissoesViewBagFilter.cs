using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SGRH.Services;

namespace SGRH.Authorization;

/// <summary>
/// Preenche ViewBag.Permissoes para todas as acções autenticadas, garantindo
/// que o menu lateral (_Layout) renderiza os itens conforme o perfil.
/// </summary>
public class PermissoesViewBagFilter : IAsyncActionFilter
{
    private readonly AutorizacaoService _autorizacao;

    public PermissoesViewBagFilter(AutorizacaoService autorizacao)
    {
        _autorizacao = autorizacao;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var idPerfilClaim = context.HttpContext.User.FindFirst("IdPerfil");
            if (idPerfilClaim != null && int.TryParse(idPerfilClaim.Value, out var idPerfil))
            {
                ((Controller)context.Controller).ViewData["Permissoes"] =
                    await _autorizacao.ObterTodasPermissoes(idPerfil);
            }
        }

        await next();
    }
}