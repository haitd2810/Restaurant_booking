using DataLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
            string userAdmin = config["Admin:username"];
            string passAdmin = config["Admin:password"];

            string name = Request.Form["name"];
            string password = Request.Form["password"]; 
            if(name.Equals(userAdmin) && passAdmin.Equals(passAdmin))
            {
                HttpContext.Session.SetString("role", "Admin");
                return Redirect("Admin/Index");
            }

            var acc =RestaurantContext.Ins.Accounts.Where(x => x.Username.Equals(name)).Include(a => a.Role).FirstOrDefault();
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
            HttpContext.Session.SetString("role", acc.Role.Name);
            HttpContext.Session.SetInt32("acc", acc.Id);

			return Redirect("/Staff/BookingInformation?page_number=1");
        }
    }
}
