using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantBooking.Pages
{
    public class activeModel : PageModel
    {
        public IActionResult OnGet()
        {
            int? id= HttpContext.Session.GetInt32("accId");
            var acc = RestaurantContext.Ins.Accounts.Find(id);
            if(acc != null)
            {
                acc.IsActive = true;
                acc.UpdateAt = DateTime.Now;
                RestaurantContext.Ins.Accounts.Update(acc);
                RestaurantContext.Ins.SaveChanges();
            }
            return Redirect("/login");
        }
    }
}
