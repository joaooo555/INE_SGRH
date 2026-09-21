using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SGRH.Services;

namespace SGRH.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class VerificarPermissaoAttribute : Attribute, IAsyncActionFilter
{
    public string Modulo { get; set; } = "";
    public string Operacao { get; set; } = "Visualizar";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var autorizacaoService = context.HttpContext.RequestServices.GetRequiredService<AutorizacaoService>();

        var idPerfilClaim = context.HttpContext.User.FindFirst("IdPerfil");
        if (idPerfilClaim == null || !int.TryParse(idPerfilClaim.Value, out var idPerfil))
        {
            context.Result = new ForbidResult();
            return;
        }

        var pode = Operacao.ToLower() switch
        {
            "visualizar" => await autorizacaoService.PodeVisualizar(idPerfil, Modulo),
            "criar" => await autorizacaoService.PodeCriar(idPerfil, Modulo),
            "editar" => await autorizacaoService.PodeEditar(idPerfil, Modulo),
            "eliminar" => await autorizacaoService.PodeEliminar(idPerfil, Modulo),
            _ => false
        };

            if (!pode)
        {
            if (IsAjaxRequest(context.HttpContext))
            {
                context.Result = new JsonResult(new { erro = true, mensagem = "Sem permissao para esta operacao." })
                {
                    StatusCode = 403
                };
            }
            else
            {
                context.Result = new RedirectResult("/Account/Login");
            }
            return;
        }

        await next();
    }

    private static bool IsAjaxRequest(HttpContext context)
    {
        return context.Request.Headers.XRequestedWith == "XMLHttpRequest"
            || context.Request.ContentType?.Contains("json") == true;
    }
}
