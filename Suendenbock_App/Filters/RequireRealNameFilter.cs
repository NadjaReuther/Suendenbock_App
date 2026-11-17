using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Suendenbock_App.Controllers;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Filters
{
    /// <summary>
    /// Wer angemeldet ist, aber noch keinen echten Namen eingetragen hat,
    /// wird beim Öffnen einer Seite einmalig nach dem Namen gefragt.
    /// Betrifft nur normale Seitenaufrufe (GET), keine APIs und keine AJAX-Aufrufe.
    /// </summary>
    public class RequireRealNameFilter : IAsyncActionFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RequireRealNameFilter(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (ShouldAskForName(context) && await HasNoNameAsync(context))
            {
                var request = context.HttpContext.Request;
                var returnUrl = request.Path + request.QueryString;
                context.Result = new RedirectToActionResult(
                    nameof(ProfilController.Name), "Profil", new { returnUrl });
                return;
            }

            await next();
        }

        private static bool ShouldAskForName(ActionExecutingContext context)
        {
            var http = context.HttpContext;

            if (http.User.Identity?.IsAuthenticated != true)
                return false;

            if (!HttpMethods.IsGet(http.Request.Method))
                return false;

            // AJAX-Aufrufe nicht umleiten
            if (http.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return false;

            // APIs nicht umleiten
            if (context.ActionDescriptor.EndpointMetadata.OfType<ApiControllerAttribute>().Any())
                return false;
            if (http.Request.Path.StartsWithSegments("/api"))
                return false;

            // Die Namensseite selbst nicht
            if (context.ActionDescriptor is ControllerActionDescriptor action &&
                action.ControllerName == "Profil")
                return false;

            return true;
        }

        private async Task<bool> HasNoNameAsync(ActionExecutingContext context)
        {
            var user = await _userManager.GetUserAsync(context.HttpContext.User);
            return user != null && string.IsNullOrWhiteSpace(user.Name);
        }
    }
}
