using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;

namespace Suendenbock_App.Controllers
{
    /// <summary>
    /// Base Controller, der von allen Controllern geerbt wird
    /// Setzt automatisch ViewBag.CurrentActId für SignalR
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Wird vor jeder Action ausgeführt
        /// Setzt ViewBag.CurrentActId mit dem aktiven Act
        /// </summary>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Nur wenn User eingeloggt ist
            if (User.Identity?.IsAuthenticated == true)
            {
                // Finde den aktiven Act
                var activeAct = await _context.Acts.FirstOrDefaultAsync(a => a.IsActive);

                if (activeAct != null)
                {
                    // WICHTIG: Verwende ActNumber (logische Nummer) statt Id (Datenbank-ID)
                    ViewBag.CurrentActId = activeAct.ActNumber;
                }
                else
                {
                    ViewBag.CurrentActId = null;
                }
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
