using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    public class AngularController : Controller
    {
        [HttpGet]
        public IActionResult AdventCalendar()
        {
            return PhysicalFile(
                Path.Combine(Directory.GetCurrentDirectory(),
                "ClientApp/dist/client-app/browser/index.html"),
                "text/html"
            );
        }
    }
}
