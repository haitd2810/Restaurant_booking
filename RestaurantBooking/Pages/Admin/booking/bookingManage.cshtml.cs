using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace RestaurantBooking.Pages.Admin.booking
{
    public class bookingManageModel : PageModel
    {
        public List<Booking> bookings { get; set; } = new List<Booking>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string Search { get; set; }
        public DateTime Date { get;set; } 
        public void OnGet(string search, DateTime? date ,int pageIndex = 1, int pageSize = 5)
        {
            CurrentPage = pageIndex;
            Search = search;
            Date = date ?? DateTime.Now;
            var query = RestaurantContext.Ins.Bookings.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Phone.Equals(search) );
            }
            if(date.HasValue)
            {
                query = query.Where(x => x.StartDate.Value==date.Value);
            }
            if (query == null)
            {
                ViewData["error"] = "Customer ";
            }
            else
            {
                int totalItems = query.Count();
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                bookings = query.Include(x => x.Table)
                            .OrderByDescending(i => i.Id)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
            }
           
        }
    }
}
