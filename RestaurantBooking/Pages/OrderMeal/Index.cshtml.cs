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
        public async Task OnGetAsync()
        {
            tableList = await _context.Tables.Where(t => t.Status == true)
                .Include(t => t.Bills.Where(b => b.Status == true)).ThenInclude(t => t.BillInfors)
                .ToListAsync();
        }
    }
}
