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

        public string date_filter { get; set; }
        public string key { get; set; }

        public string search_by { get; set; }
        public IndexModel(RestaurantContext context)
        {
            _context = context;
        }
        public List<Booking> book_infor {  get; set; }
        public async Task OnGetAsync(string page_number, string dateFilter, string keywords, string searchBy)
        {
            book_infor = await _context.Bookings.Include(b => b.Table)
                .ToListAsync();

            if (dateFilter != null) book_infor = book_infor.Where(b => b.StartDate.Value.Date.CompareTo(DateTime.Parse(dateFilter).Date) == 0).ToList();
            
            if(searchBy != null && searchBy.Length != 0)
            {
                if (searchBy == "name") book_infor = book_infor.Where(b => keywords == null? true :  b.FullName.Contains(keywords)).ToList();
                if (searchBy == "email") book_infor = book_infor.Where(b => keywords == null ? true : b.Email.Contains(keywords)).ToList();
                if (searchBy == "phone") book_infor = book_infor.Where(b => keywords == null ? true : b.Phone.Contains(keywords)).ToList();

            }
            date_filter = dateFilter ?? DateTime.Now.Date.ToString("yyyy-MM-dd");
            search_by = searchBy;
            key = keywords;

            page_current = int.Parse(page_number);
            max_page = book_infor.Count / page_size;
            if (book_infor.Count % page_size != 0) max_page += 1;

            if (max_page <= 5)
            {
                page_previous = 1;
            }
            else if (max_page > 5)
            {

                if (page_current < 3) max_page = 5;
                else if (max_page - page_current > 2) max_page = page_current + 2;

                if (max_page - page_current < 2) page_previous = max_page - 4;
                else if (page_current > 2) page_previous = page_current - 2;
            }

            book_infor = book_infor
                .OrderByDescending(b => b.StartDate)
                .OrderBy(b => b.Status)
                .Skip((int.Parse(page_number) - 1) * page_size)
                .Take(page_size)
                .ToList();
        }
    }
}
