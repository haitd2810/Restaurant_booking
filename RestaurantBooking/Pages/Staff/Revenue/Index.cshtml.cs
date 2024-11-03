using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantBooking.Pages.Revenue
{
    public class IndexModel : PageModel
    {
        public List<Booking> book_infor { get; set; }
        public double? TotalPrice { get; set; } = 0;
        public int TotalBooking { get; set; } = 0;
        public int? TotalOrder { get; set; } = 0;
        public Dictionary<string, double?> dateValueMap { get; set; } = new Dictionary<string, double?>();
        public Dictionary<Menu, int> MenuValueMap { get; set; } = new Dictionary<Menu, int>();
        private readonly RestaurantContext _context;

        public IndexModel(RestaurantContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetString("role") == null ||
                HttpContext.Session.GetString("role") != "staff") return Redirect("/Restaurant");

            List<Bill> bill_List = await _context.Bills.Where(b => b.Payed == true 
                && b.UpdateAt.Value.Date.CompareTo(DateTime.Now.Date) == 0)
                .Include(b => b.BillInfors).ToListAsync();
            GetStaticData(bill_List);

            List<Booking> book_list = await _context.Bookings.Where(b => b.StartDate.Value.Date.CompareTo(DateTime.Now.Date) == 0)
                .ToListAsync();
            TotalBooking = book_list.Count;

            List<Bill> all_bill = await _context.Bills.Where(b => b.Payed == true
                && b.UpdateAt.Value.Date.CompareTo(DateTime.Now.AddDays(1).AddMonths(-1).Date) >= 0)
                .Include(b => b.BillInfors).ToListAsync();

            UpdateDataChart(all_bill);

            List<Menu> menu_List = await _context.Menus.Include(m => m.Cate).Where(m => m.DeleteFlag == false).ToListAsync();
            GetDataTrendMeal(bill_List, menu_List);
            book_infor = await _context.Bookings.Include(b => b.Table)
                .Where(b => b.StartDate.Value.Date.CompareTo(DateTime.Now.Date) == 0)
                .ToListAsync();
            return Page();

        }
        private void GetStaticData(List<Bill> bill_List)
        {
            foreach (var bill in bill_List)
            {
                foreach (var billInfor in bill.BillInfors)
                {
                    TotalPrice += billInfor.Price;
                    TotalOrder += billInfor.Quantity;
                }
                
            }
        }
        private void UpdateDataChart(List<Bill> all_bill)
        {
            foreach (var bill in all_bill)
            {
                string billDate = bill.UpdateAt.Value.Date.ToString().Split(" ")[0];
                double? currentTotal = 0;
                if (dateValueMap.ContainsKey(billDate))
                {
                    currentTotal = dateValueMap[billDate];
                }
                foreach (BillInfor b in bill.BillInfors)
                {
                    currentTotal += b.Price;
                }
                dateValueMap[billDate] = currentTotal;
            }
            if (!dateValueMap.ContainsKey(DateTime.Now.Date.ToString().Split(" ")[0]))
            {
                dateValueMap.Add(DateTime.Now.Date.ToString().Split(" ")[0], 0);
            }
            dateValueMap = dateValueMap
                            .OrderBy(entry => DateTime.Parse(entry.Key))
                            .ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        private void GetDataTrendMeal(List<Bill> bill_List, List<Menu> menu_List)
        {
            foreach (var bill in bill_List)
            {
                foreach (var bill_infor in bill.BillInfors)
                {
                    Menu menu = menu_List.Where(m => m.Id == bill_infor.MenuId).FirstOrDefault();
                    if (menu != null)
                    {
                        int count = 1;
                        if (MenuValueMap.ContainsKey(menu))
                        {
                            count = MenuValueMap[menu] + 1;
                        }
                        MenuValueMap[menu] = count;
                    }
                }
            }

            MenuValueMap = MenuValueMap.Where(entry => entry.Value > 1).OrderByDescending(entry => entry.Value)
                            .ToDictionary(entry => entry.Key, entry => entry.Value);
        }
        
    }
}
