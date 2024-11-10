using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Hubs;

namespace RestaurantBooking.Pages.Cooker.Ingredient
{
    public class EditModel : PageModel
    {
        private readonly RestaurantContext _context;
        public EditModel(RestaurantContext context)
        {
            _context = context;
        }
        public IActionResult OnGet(int id)
        {
            if(id == null) return Redirect("/Cooker/Ingredient/Index");
            return Redirect("/Cooker/Ingredient/Index");
        }
        public async  Task<IActionResult> OnPost(int id, string name, string weight, string price)
        {
            DataLibrary.Models.Ingredient ing = await _context.Ingredients.FirstOrDefaultAsync( i => i.Id == id);
            if(ing != null)
            {
                ing.Name = name;
                ing.Weight = double.Parse(weight);
                ing.Price = double.Parse(price);
                ing.UpdateAt = DateTime.Now;
                _context.Ingredients.Update(ing);
                await _context.SaveChangesAsync();
            }
            return Redirect("/Cooker/Ingredient/Index");
        }
    }
}
