using DataLibrary.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Hubs;
using System.Drawing.Printing;

namespace RestaurantBooking.Pages.Cooker.Menu
{
    public class EditModel : PageModel
    {
        private readonly IHubContext<HubServer> _hub;
        private readonly RestaurantContext _context;
        public EditModel(IHubContext<HubServer> _hub, RestaurantContext context)
        {
            this._hub = _hub;
            _context = context;
        }
        public List<Category> cateList {  get; set; }
        public DataLibrary.Models.Menu menu { get; set; }
        public async Task OnGetAsync(string id)
        {
            cateList = await _context.Categories.Where(c => c.DeleteFlag == false).ToListAsync();
            menu = await _context.Menus.Where(m => m.Id.ToString() == id).FirstOrDefaultAsync();
        }

        public async Task<IActionResult> OnPostAsync(int? menuID, string name, double? price, int? quantity, IFormFile img, int? categoryID, string? details)
        {
            if(img != null)
            {
                var path = Path.GetExtension(img.FileName).ToLower();
                if (path != ".png" && path != ".jpg" && path != ".jpeg")
                {
                    TempData["Failed_Edit"] = "Please upload file images";
                    return Redirect("/Cooker/Menu/Edit?id=" + menuID);
                }
            }
            
            DataLibrary.Models.Menu m = await _context.Menus.Where(x => x.Id == menuID).FirstOrDefaultAsync();
            if (m != null) {
                m.Name = name;
                m.Detail = details;
                m.Price = price;
                m.Quantity = quantity;
                m.CateId = categoryID;
                m.UpdateAt = DateTime.Now;
                if (img != null) {
                    m.Img = "/assets/img/" + img.FileName;
                }
                _context.Menus.Update(m);
                await _context.SaveChangesAsync();
            }
            if(img != null && img.Length > 0) {
                var filePath = Path.Combine("wwwroot/assets/img/", img.FileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }
            }
            _hub.Clients.All.SendAsync("LoadAll");
            return Redirect("/Cooker/Menu/Edit?id="+menuID);
        }
    }
}
