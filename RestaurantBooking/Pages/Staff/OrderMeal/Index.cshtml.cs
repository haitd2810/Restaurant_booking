using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Hubs;

namespace RestaurantBooking.Pages.OrderMeal
{
    public class IndexModel : PageModel
    {
        public List<Table> tableList {  get; set; }
        private readonly RestaurantContext _context;
        public IndexModel(RestaurantContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("role") == null ||
                HttpContext.Session.GetString("role") != "staff") return Redirect("/Restaurant");

            tableList = await _context.Tables.Where(t => t.DeleteFlag == false)
                .Include(t => t.Bills.Where(b => b.Status == true)).ThenInclude(t => t.BillInfors)
                .ToListAsync();
            return Page();
        }
    }
}
