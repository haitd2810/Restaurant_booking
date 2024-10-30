using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantBooking.Pages.Admin.table
{
    public class tableManageModel : PageModel
    {
        public List<Table> tables { get; set; } = new List<Table>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string Search { get; set; }
        public void OnGet(string search, int pageIndex = 1, int pageSize = 5)
        {
            CurrentPage = pageIndex;
            Search = search;
            var query = RestaurantContext.Ins.Tables.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Id == int.Parse(search));
            }
            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            tables = query
                        .OrderByDescending(i => i.Id)
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }
    }
}
