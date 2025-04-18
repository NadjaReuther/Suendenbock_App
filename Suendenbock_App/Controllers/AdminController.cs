using Microsoft.AspNetCore.Mvc;

namespace Suendenbock_App.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
