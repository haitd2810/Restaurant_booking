using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;

namespace RestaurantBooking.Pages.BookingInformation
{
    public class IndexModel : PageModel
    {
        private readonly RestaurantContext _context;
        private readonly int page_size = 10;
        public int page_current { get; set; } = 1;
        public int max_page { get; set; } = 0;
        public int page_previous { get; set; } = 1;
        public IndexModel(RestaurantContext context)
        {
            _context = context;
        }
        public List<Booking> book_infor {  get; set; }
        public async Task OnGetAsync(string page_number)
        {
            book_infor = await _context.Bookings.Include(b => b.Table)
                .ToListAsync();
            page_current = int.Parse(page_number);
            max_page = book_infor.Count/page_size;
            if (book_infor.Count % page_size != 0) max_page += 1;
            
            if(max_page <= 5 )
            {
                page_previous = 1;
            }
            else if (max_page > 5)
            {

                if (page_current < 3) max_page = 5;
                else if(max_page - page_current > 2) max_page = page_current + 2;

                if (max_page - page_current < 2) page_previous = max_page - 4;
                else if (page_current > 2) page_previous = page_current - 2;
            }

            book_infor = book_infor
                .Skip((int.Parse(page_number) - 1) * page_size)
                .Take(page_size).ToList();
        }
    }
}
