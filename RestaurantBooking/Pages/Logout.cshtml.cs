using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantBooking.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            if(HttpContext.Session.GetString("acc") != null)
            {
                HttpContext.Session.Remove("acc");
                HttpContext.Session.Remove("role");
            }

            return Redirect("/Restaurant");
        }
    }
}
