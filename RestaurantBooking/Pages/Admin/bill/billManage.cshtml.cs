using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace RestaurantBooking.Pages.Admin.bill
{
    public class billManageModel : PageModel
    {   
        public List<Bill> Bills { get; set; }= new List<Bill>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string Search { get; set; }
        public string Status { get; set; }
        public DateTime To { get; set; }
        public DateTime From { get; set; }
        //, DateTime? to, DateTime? from
        public IActionResult OnGet(string search, string status, int pageIndex = 1, int pageSize = 5)
        {
            if (HttpContext.Session.GetString("role") == null ||
                HttpContext.Session.GetString("role") != "Admin") return Redirect("/Restaurant");
            CurrentPage = pageIndex;
            Search = search;
            //To = to ?? DateTime.Now ;
            //From = from ?? DateTime.Now ;
            Status = status;
            var query = RestaurantContext.Ins.Bills.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int searchId))
                {
                    query = query.Where(x => x.Id == searchId);
                }
            }
            //if (from.HasValue && to.HasValue)
            //{
            //    query = query.Where(x => (x.CreateAt >= from && x.CreateAt <= to) );
            //}

            if (!string.IsNullOrEmpty(status) && !status.Equals("All"))
            {
                if (bool.TryParse(status, out bool statusValue))
                {
                    query = query.Where(x => x.Status == statusValue);
                }
            }
            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            Bills = query.Include(x => x.Table)
                         .OrderByDescending(i => i.Id)
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();
            return Page();
        }

    }
}
