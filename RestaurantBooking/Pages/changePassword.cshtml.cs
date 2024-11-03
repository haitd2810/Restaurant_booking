using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantBooking.Pages
{
	public class changePasswordModel : PageModel
	{
		public IActionResult OnGet()
		{
			if (HttpContext.Session.GetString("role") == null)
				return Redirect("/Restaurant");
			return Page();
		}

		public IActionResult OnPost()
		{
			string old = Request.Form["old"];
			string newpass = Request.Form["new"];
			string comfirm = Request.Form["comfirm"];
			int id = (int)HttpContext.Session.GetInt32("acc");
			var acc = RestaurantContext.Ins.Accounts.Find(id);
			if(acc != null)
			{
				bool pass = BCrypt.Net.BCrypt.Verify(old, acc.Password);
				if(!pass)
				{
					ViewData["error"] = "Password not correct";
					return Page();
				}
				if (!newpass.Equals(comfirm))
				{
					ViewData["error"] = "Comfirm password not correct";
					return Page();
				}
				string hash = BCrypt.Net.BCrypt.HashPassword(newpass);
				acc.Password=hash;
				RestaurantContext.Ins.Accounts.Update(acc);
				RestaurantContext.Ins.SaveChanges();
			}
			return Redirect("Staff/OrderMeal");
		}
	}
}
