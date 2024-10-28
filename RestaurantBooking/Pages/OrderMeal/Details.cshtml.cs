using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RestaurantBooking.Hubs;
using System.Runtime.CompilerServices;

namespace RestaurantBooking.Pages.OrderMeal
{
    public class DetailsModel : PageModel
    {
        private readonly IHubContext<HubServer> _hub;
        private readonly RestaurantContext _context;
        public DetailsModel(IHubContext<HubServer> _hub, RestaurantContext context)
        {
            this._hub = _hub;
            _context = context;
        }

        public string TableID { get; set; }
        public Bill billTable { get; set; }
        public List<BillInfor> billDetail { get; set; }
        public List<Menu> menu_list { get; set; }
        public List<Category> category_list { get; set; }
        public async Task OnGetAsync(string id)
        {
            TableID = id;
            billTable = await _context.Bills.Where(b => b.TableId == int.Parse(id))
                .Where(b => b.Status == true)
                .FirstOrDefaultAsync();
            if (billTable != null) billDetail = await _context.BillInfors.Where(bi => bi.BillId == billTable.Id && bi.Status == true)
                    .Include(bd => bd.Menu).Include(bd => bd.Bill).ToListAsync();
            menu_list = await _context.Menus.Include(m => m.Cate).Where(m => m.IsSell == true).ToListAsync();
            category_list = await _context.Categories.Where(ct => ct.IsActive == true).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string billId, string tableId)
        {
            if (billId != null && tableId != null)
            {
                billTable = await _context.Bills.Where(b => b.Id == int.Parse(billId))
                .Where(b => b.Status == true)
                .FirstOrDefaultAsync();
                if (billTable != null)
                {
                    billTable.Status = false;
                    billTable.Payed = true;
                    _context.Bills.Update(billTable);
                    await _context.SaveChangesAsync();
                }
            }
            _hub.Clients.All.SendAsync("LoadAll");
            return Redirect("/OrderMeal");
        }
    }
}
