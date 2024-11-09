using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RestaurantBooking.Hubs;

namespace RestaurantBooking.Pages.Cooker.Menu
{
    public class DeleteModel : PageModel
    {
        private readonly IHubContext<HubServer> _hub;
        private readonly RestaurantContext _context;
        public DeleteModel(IHubContext<HubServer> _hub, RestaurantContext context)
        {
            this._hub = _hub;
            _context = context;
        }
        public async Task<IActionResult> OnGet(int id)
        {
            DataLibrary.Models.Menu menu = await _context.Menus.FindAsync(id);
            if (menu != null) {
                if(menu.DeleteFlag == false)
                {
                    menu.DeleteFlag = true;
                    menu.DeleteAt = DateTime.Now;
                }
                else
                {
                    menu.DeleteFlag = false;
                    menu.UpdateAt = DateTime.Now;
                    menu.DeleteAt = null;
                }
                _context.Menus.Update(menu);
                await _context.SaveChangesAsync();
            }
            _hub.Clients.All.SendAsync("LoadAll");
            return Redirect("/Cooker/Menu/Index");
        }
    }
}
