using Microsoft.AspNetCore.Mvc;

namespace Suendenbock_App.Controllers
{
    public class MagicClassController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
