using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using RestaurantBooking.Hubs;

namespace RestaurantBooking.Pages.Cooker.Menu
{
    public class IndexModel : PageModel
    {
        private readonly IHubContext<HubServer> _hub;
        private readonly RestaurantContext _context;
        public IndexModel(IHubContext<HubServer> _hub, RestaurantContext context)
        {
            this._hub = _hub;
            _context = context;
        }
        public List<DataLibrary.Models.Menu> menu_list { get; set; }
        public List<Category> category_list { get; set; }
        public async Task OnGet()
        {
            category_list = await _context.Categories.Where(ct => ct.DeleteFlag == false).ToListAsync();
            menu_list = await _context.Menus.Include(m => m.Cate).ToListAsync();
        }
    }
}
