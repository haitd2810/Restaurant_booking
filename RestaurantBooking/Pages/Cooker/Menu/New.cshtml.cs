using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Hubs;

namespace RestaurantBooking.Pages.Cooker.Menu
{
    public class NewModel : PageModel
    {
        private readonly IHubContext<HubServer> _hub;
        private readonly RestaurantContext _context;
        public NewModel(IHubContext<HubServer> _hub, RestaurantContext context)
        {
            this._hub = _hub;
            _context = context;
        }
        public List<Category> cateList { get; set; }
        public async Task OnGet()
        {
            cateList = await _context.Categories.Where(c => c.DeleteFlag == false).ToListAsync();
        }

        public async Task<IActionResult> OnPostAddNewAsync(string name, double? price, int? quantity, IFormFile img, int? categoryID, string? details)
        {
            var path = Path.GetExtension(img.FileName).ToLower();
            if (path != ".png" && path != ".jpg" && path != ".jpeg")
            {
                TempData["Failed_Add"] = "Please upload file images";
                return Redirect("/Cooker/Menu/New");
            }

            DataLibrary.Models.Menu m = new DataLibrary.Models.Menu();
            m.Name = name;
            m.Detail = details;
            m.Price = price;
            m.Quantity = quantity;
            m.CateId = categoryID;
            m.CreateAt = DateTime.Now;
            m.DeleteFlag = false;
            m.Img = "/assets/img/" + img.FileName;
            _context.Menus.Add(m);
            await _context.SaveChangesAsync();
            var filePath = Path.Combine("wwwroot/assets/img/", img.FileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await img.CopyToAsync(stream);
            }
            return Redirect("/Cooker/Menu/Index");
        }
    }
}
