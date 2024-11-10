using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace RestaurantBooking.Pages.Cooker.Ingredient
{
    public class NewModel : PageModel
    {
        private readonly RestaurantContext _context;
        public NewModel(RestaurantContext context)
        {
            _context = context;
        }
        public IActionResult OnGet()
        {
            return Redirect("/Cooker/Ingredient/Index");
        }
        public async Task<IActionResult> OnPost(string name, string weight, string price)
        {
            DataLibrary.Models.Ingredient ing = new DataLibrary.Models.Ingredient();
            ing.Name = name;
            ing.Weight = double.Parse(weight);
            ing.Price = double.Parse(price);
            ing.CreateAt = DateTime.Now;
            _context.Ingredients.Update(ing);
            await _context.SaveChangesAsync();
            return Redirect("/Cooker/Ingredient/Index");
        }
    }
}
