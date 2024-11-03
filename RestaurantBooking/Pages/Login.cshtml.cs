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
			IConfiguration config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			string accAdmin = config["Admin:username"];
			string passFrom = config["Admin:password"];
            string role = config["Admin:Role"];
			string name = Request.Form["name"];
            string password = Request.Form["password"]; 
            if(name.Equals(accAdmin) && passFrom.Equals(passFrom))
            {
				return Redirect("Admin/Index");
			}
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

			return Redirect("Restaurant/Index");
        }
    }
}
