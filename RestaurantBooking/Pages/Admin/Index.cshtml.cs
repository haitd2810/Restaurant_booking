using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json;

namespace RestaurantBooking.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public List<Bill> bills { get; set; } = new List<Bill>();
        public List<BillInfor> billsInfor { get; set; } = new List<BillInfor>();
        public List<Menu> menus { get; set; } = new List<Menu>();
        public List<Account> accounts { get; set; } = new List<Account>();
        public List<Ingredient> ingredients { get; set; } = new List<Ingredient>();

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("role") == null ||
                HttpContext.Session.GetString("role") != "Admin") return Redirect("/Restaurant");
            GetDataTrendMeal();
            getStaff();
            getDataReport();
            GetTotal();
            return Page();

        }

        public void GetTotal()
        {
            float sell = 0;
            var bills = RestaurantContext.Ins.Bills
                    .Where(x => x.CreateAt >= DateTime.Today && x.CreateAt < DateTime.Today.AddDays(1) && x.Payed == true)
                    .ToList();
            foreach (var bill in bills)
            {
                var billInfos = RestaurantContext.Ins.BillInfors.Where(x => x.BillId == bill.Id).ToList();
                foreach (var billInfo in billInfos)
                {
                    sell += (float)(billInfo.Price);
                }
            }
            float original = 0;
            var ingredients = RestaurantContext.Ins.Ingredients
              .Where(x => x.UpdateAt >= DateTime.Today && x.UpdateAt < DateTime.Today.AddDays(1))
              .ToList();
            foreach (var ingredient in ingredients)
            {
                original += (float)ingredient.Price;
            }
            sell = sell - original;
            ViewData["sell"] = sell;
        }

        private void GetDataTrendMeal()
        {
            bills = RestaurantContext.Ins.Bills.ToList();
            billsInfor = RestaurantContext.Ins.BillInfors.ToList();
            menus = RestaurantContext.Ins.Menus.ToList();

            var trend = from b in bills
                        where b.CreateAt >= DateTime.Today && b.CreateAt < DateTime.Today.AddDays(1)
                        join bi in billsInfor on b.Id equals bi.BillId
                        join m in menus on bi.MenuId equals m.Id
                        group new { bi, m } by new { m.Id, m.Name, m.Img, m.Quantity } into grouped
                        orderby grouped.Count() descending
                        select new
                        {
                            Id = grouped.Key.Id,
                            Name = grouped.Key.Name,
                            Img = grouped.Key.Img,
                            QuantitySold = grouped.Count(),
                            Instock = grouped.Key.Quantity - grouped.Count(),
                            TotalBills = grouped.Select(g => g.bi.BillId).Distinct().Count()
                        };

            ViewData["trend"] = trend;
        }

        private void getDataReport()
        {
            var dailyData = new Dictionary<string, (float original, float sell)>();

            for (int i = -6; i <= 0; i++)
            {
                var date = DateTime.Today.AddDays(i);
                var formattedDate = date.ToString("dd/MM/yyyy");

                Console.WriteLine($"Ngày đã định dạng: {formattedDate}");

                var ingredients = RestaurantContext.Ins.Ingredients
           .Where(x => x.UpdateAt >= date && x.UpdateAt < date.AddDays(1))
           .ToList();

                var bills = RestaurantContext.Ins.Bills
                    .Where(x => x.CreateAt >= date && x.CreateAt < date.AddDays(1) && x.Payed == true)
                    .ToList();

                Console.WriteLine($"Số lượng Ingredients: {ingredients.Count}, Số lượng Bills: {bills.Count}");

                float original = 0;
                float sell = 0;

                foreach (var ingredient in ingredients)
                {
                    original += (float)ingredient.Price;
                }


                foreach (var bill in bills)
                {
                    var billInfos = RestaurantContext.Ins.BillInfors.Where(x => x.BillId == bill.Id).ToList();
                    foreach (var billInfo in billInfos)
                    {
                        sell += (float)(billInfo.Price);
                    }
                }

                dailyData[formattedDate] = (original, sell);
            }

            ViewData["dailyData"] = JsonConvert.SerializeObject(dailyData);
        }


        public void getStaff()
        {
            bills = RestaurantContext.Ins.Bills.ToList();
            accounts = RestaurantContext.Ins.Accounts.ToList();

            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var staff = from b in bills
                        where b.CreateAt >= startOfMonth && b.CreateAt < startOfNextMonth
                        join a in accounts on b.CreateBy equals a.Id
                        group b by a.Code into grouped
                        orderby grouped.Count() descending
                        select new
                        {
                            Code = grouped.Key,
                            Quantity = grouped.Count()
                        };
            ViewData["staff"] = staff;
        }

    }
}
