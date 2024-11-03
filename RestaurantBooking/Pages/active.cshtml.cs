using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantBooking.Pages
{
    public class activeModel : PageModel
    {
        public IActionResult OnGet(string token)
        {   
            if(string.IsNullOrEmpty(token))
            {
				TempData["Message"] = "Token not existed";
				return Redirect("/login");
			}
            var t = RestaurantContext.Ins.Tokens.Where(x => x.Token1.Equals(token)).FirstOrDefault();
            if (t != null)
            {
                var acc = RestaurantContext.Ins.Accounts.Find(t.AccountId);
                if (acc != null)
                {
                    acc.IsActive = true;
                    acc.UpdateAt = DateTime.Now;
                    RestaurantContext.Ins.Accounts.Update(acc);
                    RestaurantContext.Ins.SaveChanges();
                }
                RestaurantContext.Ins.Tokens.Remove(t);
                RestaurantContext.Ins.SaveChanges();
            }
            TempData["Message"] = "Avtive account successfull";
            return Redirect("/login");
        }
    }
}
