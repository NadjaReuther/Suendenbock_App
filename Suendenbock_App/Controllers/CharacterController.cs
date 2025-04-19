using Microsoft.AspNetCore.Mvc;

namespace Suendenbock_App.Controllers
{
    public class CharacterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateForm()
        {
            return View();
        }
    }
}
