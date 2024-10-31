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
            return Redirect("/OrderMeal");
        }

        public async Task< IActionResult> OnPostAsync(List<string> items_id, List<string> quantity, string tableId, string billId)
        {
            for(int i = 0; i < items_id.Count; i++)
            {
                BillInfor bill_infor = await _context.BillInfors
                    .Where(bi => bi.Id.ToString() == items_id[i]).FirstOrDefaultAsync();
                if (bill_infor == null) continue;
                else
                {
                    if (bill_infor.Quantity.ToString() == quantity[i]) continue;
                    else
                    {
                        if (int.Parse(quantity[i]) <= 0)
                        {
                            _context.BillInfors.Remove(bill_infor);
                        }
                        else
                        {
                            bill_infor.UpdateAt = DateTime.Now;
                            bill_infor.Quantity = int.Parse(quantity[i]);
                            bill_infor.Price = int.Parse(quantity[i]) * bill_infor.Menu.Price;
                            _context.BillInfors.Update(bill_infor);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
            }
            List<BillInfor> billInf = await _context.BillInfors.Where(b => b.BillId.ToString() == billId).ToListAsync();
            if(billInf == null || billInf.Count == 0)
            {
                Bill bill = await _context.Bills.Where(b => b.Id.ToString() == billId).FirstAsync();
                bill.Status = false;
                bill.UpdateAt = DateTime.Now;
                _context.Bills.Update(bill);
                await _context.SaveChangesAsync();
            }
            _hub.Clients.All.SendAsync("LoadAll");
            return Redirect("/OrderMeal/Details?id=" + tableId);
        }
    }
}
