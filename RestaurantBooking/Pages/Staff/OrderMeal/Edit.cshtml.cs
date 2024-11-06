using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Hubs;

namespace RestaurantBooking.Pages.OrderMeal
{
    public class EditModel : PageModel
    {
        private readonly IHubContext<HubServer> _hub;
        private readonly RestaurantContext _context;
        public EditModel(IHubContext<HubServer> hub, RestaurantContext context)
        {
            _hub = hub;
            _context = context;
        }
        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetString("role") == null ||
                HttpContext.Session.GetString("role") != "staff") return Redirect("/Restaurant");
            return Redirect("/Staff/BookingInformation?page_number=1");
        }

        public async Task< IActionResult> OnPostAsync(List<string> items_id, List<string> quantity, string tableId, string billId)
        {
            if (HttpContext.Session.GetString("role") == null ||
                HttpContext.Session.GetString("role") != "staff") return Redirect("/Restaurant");

            for (int i = 0; i < items_id.Count; i++)
            {
                BillInfor bill_infor = await _context.BillInfors
                    .Where(bi => bi.Id.ToString() == items_id[i]).Include(bi => bi.Menu).FirstOrDefaultAsync();
                if (bill_infor == null) continue;
                else
                {
                    if (bill_infor.Quantity.ToString() == quantity[i]) continue;
                    else
                    {
                        Menu menu = await _context.Menus.Where(m => m.Id == bill_infor.MenuId).FirstOrDefaultAsync();
                        if (int.Parse(quantity[i]) <= 0)
                        {
                            
                            menu.Quantity += bill_infor.Quantity;
                            _context.BillInfors.Remove(bill_infor);
                        }
                        else
                        {
                            int difference =  (int)bill_infor.Quantity - int.Parse(quantity[i]);
                            menu.Quantity += difference;
                            bill_infor.UpdateAt = DateTime.Now;
                            bill_infor.Quantity = int.Parse(quantity[i]);
                            bill_infor.Price = int.Parse(quantity[i]) * bill_infor.Menu.Price;
                            _context.BillInfors.Update(bill_infor);
                        }
                        _context.Menus.Update(menu);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            List<BillInfor> billInf = await _context.BillInfors.Where(b => b.BillId.ToString() == billId).ToListAsync();
            if(billInf == null || billInf.Count == 0)
            {
                Bill bill = await _context.Bills.Where(b => b.Id.ToString() == billId).FirstAsync();
                _context.Bills.Remove(bill);
                await _context.SaveChangesAsync();
            }
            _hub.Clients.All.SendAsync("LoadAll");
            return Redirect("/Staff/OrderMeal/Details?id=" + tableId);
        }
    }
}
