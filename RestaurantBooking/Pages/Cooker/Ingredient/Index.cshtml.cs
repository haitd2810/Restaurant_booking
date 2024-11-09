using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Hubs;

namespace RestaurantBooking.Pages.Cooker.Ingredient
{
    public class IndexModel : PageModel
    {
        private readonly RestaurantContext _context = new RestaurantContext();
        public List<DataLibrary.Models.Ingredient> ingredients { get; set; }
        public async Task OnGet()
        {
            ingredients = await _context.Ingredients.ToListAsync();
        }
    }
}
