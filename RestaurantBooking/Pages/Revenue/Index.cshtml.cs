using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace RestaurantBooking.Pages.Revenue
{
    public class IndexModel : PageModel
    {
        public double? TotalPrice { get; set; } = 0;
        public int TotalBooking { get; set; } = 0;
        public int TotalOrder { get; set; } = 0;
        private readonly RestaurantContext _context;

        public IndexModel(RestaurantContext context)
        {
            _context = context;
        }
        public async Task OnGetAsync()
        {
            List<Bill> bill_List = await _context.Bills.Where(b => b.Payed == true 
                && b.CreateAt.Value.Date.CompareTo(DateTime.Now.Date) == 0)
                .Include(b => b.BillInfors).ToListAsync();
            foreach (var bill in bill_List)
            {
                foreach (var billInfor in bill.BillInfors)
                {
                    TotalPrice += billInfor.Price;
                }
                TotalOrder += bill.BillInfors.Count;
            }

            List<Booking> book_list = await _context.Bookings.Where(b => b.StartDate.Value.Date.CompareTo(DateTime.Now.Date) == 0)
                .ToListAsync();
            TotalBooking = book_list.Count;
            
        }
    }
}
