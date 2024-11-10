using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace RestaurantBooking.Pages.Admin.bill
{
    public class billManageModel : PageModel
    {   
        public List<Bill> Bills { get; set; }= new List<Bill>();
        public List<Account> Account { get; set; } = new List<Account>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string Search { get; set; }
        public string Status { get; set; }
        public string Code { get; set; }
        public DateTime To { get; set; }
        public DateTime From { get; set; }
        //, DateTime? to, DateTime? from
        public IActionResult OnGet(string search, string code, string status, int pageIndex = 1, int pageSize = 5)
        {
            CurrentPage = pageIndex;
            Search = search;
            Code = code;
            Status = status;

            var query = RestaurantContext.Ins.Bills.AsQueryable();

            if (!string.IsNullOrEmpty(search) && int.TryParse(search, out int searchId))
            {
                query = query.Where(x => x.Id == searchId);
            }

            if (!string.IsNullOrEmpty(code) && code != "All" && int.TryParse(code, out int accountId))
            {
                query = query.Where(x => x.CreateBy == accountId);
            }

            if (!string.IsNullOrEmpty(status) && status != "All" && bool.TryParse(status, out bool statusValue))
            {
                query = query.Where(x => x.Status == statusValue);
            }

            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            Bills = query.Include(x => x.Table)
                         .OrderByDescending(i => i.Id)
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

            Account = RestaurantContext.Ins.Accounts.ToList();

            return Page();
        }


    }
}
