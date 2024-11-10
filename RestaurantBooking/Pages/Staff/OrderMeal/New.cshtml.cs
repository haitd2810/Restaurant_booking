using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Hubs;

namespace RestaurantBooking.Pages.OrderMeal
{
    public class NewModel : PageModel
    {
        
        private readonly IHubContext<HubServer> _hub;
        private readonly RestaurantContext _context;
        public NewModel(IHubContext<HubServer> hub, RestaurantContext context)
        {
            _hub = hub;
            _context = context;
        }
        public Bill billTable { get; set; }
        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetString("role") == null ||
                HttpContext.Session.GetString("role") != "staff") return Redirect("/Restaurant");

            return Redirect("/Staff/OrderMeal");
        }

        public async Task<IActionResult> OnPostAsync(List<string> MenuId, string tableId)
        {
            if (HttpContext.Session.GetString("role") == null ||
                HttpContext.Session.GetString("role") != "staff") return Redirect("/Restaurant");

            int billId = 0;
            billTable = await _context.Bills.Where(b => b.TableId == int.Parse(tableId))
                .Where(b => b.Status == true)
                .FirstOrDefaultAsync();
            if (billTable == null)
            {
                Bill bill = new Bill();
                bill.TableId = int.Parse(tableId);
                bill.Payed = false;
                bill.CreateAt = DateTime.Now;
                bill.Status = true;
                if (HttpContext.Session.GetInt32("acc") != null) bill.CreateBy = HttpContext.Session.GetInt32("acc");
                
                try
                {
                    _context.Bills.Add(bill);
                    await _context.SaveChangesAsync();
                    billId = bill.Id;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }
            else
            {
                billId = billTable.Id;
            }
            try
            {
                foreach (string item in MenuId)
                {
                    int menuID = int.Parse(item.Split("-")[0]);
                    Menu menu = await _context.Menus.Where(m => m.Id == menuID).FirstOrDefaultAsync();
                    if (menu == null) throw new Exception();

                    int quantity = int.Parse(item.Split("-")[1]);

                    BillInfor billInfor = await _context.BillInfors
                        .Where(m => m.BillId == billId && m.MenuId == menuID).FirstOrDefaultAsync();

                    if (billInfor == null)
                    {
                        billInfor = new BillInfor();
                        billInfor.BillId = billId;
                        billInfor.Quantity = quantity;
                        billInfor.MenuId = menuID;
                        billInfor.Price = menu.Price * quantity;
                        billInfor.CreateAt = DateTime.Now;
                        _context.BillInfors.Add(billInfor);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        billInfor.Quantity = billInfor.Quantity + quantity;
                        billInfor.Price = billInfor.Price + menu.Price * quantity;
                        billInfor.UpdateAt = DateTime.Now;
                        _context.BillInfors.Update(billInfor);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            _hub.Clients.All.SendAsync("LoadAll");
            return Redirect("/Staff/OrderMeal/Details?id=" + tableId);
        }
    }
}
