using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopCustomer.Pages.CoffeePage
{
    public class IndexModel : PageModel
    {
        public List<Menu> menu_list { get; set; }
        public List<Category> category_list { get; set; }
        public void OnGet()
        {
            menu_list = CoffeeShopContext.ins.Menus.Include(m => m.Cate).ToList();
            category_list = CoffeeShopContext.ins.Categories.ToList();
        }
    }
}
