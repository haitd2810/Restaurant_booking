using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantBooking.Pages
{
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            string name = Request.Form["name"];
            string password = Request.Form["password"]; 
            var acc =RestaurantContext.Ins.Accounts.Where(x => x.Username.Equals(name)).FirstOrDefault();
            if(acc == null)
            {
                ViewData["error"] = "Account not existed";
                return Page();
            }
			bool pass = BCrypt.Net.BCrypt.Verify(password, acc.Password);
            if (!pass)
            {
				ViewData["errorP"] = "Password not correct";
				return Page();
			}
            if (acc.IsActive == false)
            {
                ViewData["errorA"] = "Account is not active";
                return Page();
            }
            HttpContext.Session.SetInt32("acc", acc.Id);

			return Redirect("Admin/Index");
        }
    }
}
