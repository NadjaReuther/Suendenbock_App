using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Suendenbock_App.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Registrierung ist deaktiviert
            return Page();
        }

        public IActionResult OnPost()
        {
            // Registrierung ist komplett deaktiviert
            ModelState.AddModelError(string.Empty, "Registrierung ist nicht verfügbar.");
            return Page();
        }
    }
}
