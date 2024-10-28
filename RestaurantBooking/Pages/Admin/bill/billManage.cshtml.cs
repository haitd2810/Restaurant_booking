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
        public void OnGet(int pageIndex = 1, int pageSize = 5)
        {
            CurrentPage = pageIndex;
            var query = RestaurantContext.Ins.Bills.AsQueryable();
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
