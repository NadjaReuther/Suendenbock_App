using Microsoft.AspNetCore.Mvc;

namespace Suendenbock_App.Controllers
{
    public class GuildController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
