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
        public void OnGet(string search, int pageIndex = 1, int pageSize = 5)
        {
            CurrentPage = pageIndex;
            Search = search;
            var query = RestaurantContext.Ins.Bills.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {   
                query = query.Where(x => x.Id == int.Parse(search));
            }
            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            Bills = query.Include(x => x.Table)
                        .OrderByDescending(i => i.Id)
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }
    }
}
