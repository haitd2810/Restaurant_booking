using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

namespace RestaurantBooking.Pages.Admin.category
{
    public class cateManageModel : PageModel
    {
        public List<Category> categories { get; set; } = new List<Category>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string Search { get; set; }
        public void OnGet(string search, int pageIndex = 1, int pageSize = 5)
        {
            CurrentPage = pageIndex;
            Search = search;
            var query = RestaurantContext.Ins.Categories.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }
            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            categories = query
                        .OrderByDescending(i => i.Id)
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }

        public IActionResult OnPostAdd()
        {
            string name = Request.Form["name"];
            var cate = RestaurantContext.Ins.Categories.Where(x => x.Name.Equals(name)).FirstOrDefault();
            if (cate != null)
            {
                return Page();
            }
            else
            {
                Category c = new Category
                {
                    Name = name,
                    CreateAt=DateTime.Now,
                    DeleteFlag = true
                };
                RestaurantContext.Ins.Categories.Add(c);
                RestaurantContext.Ins.SaveChanges();
                return RedirectToPage("/Admin/category/cateManage");
            }
        }

        public IActionResult OnPostDelete()
        {   
            string id = Request.Form["itemId"];
            var cate = RestaurantContext.Ins.Categories.Find(int.Parse(id));
            if (cate != null)
            {
                cate.DeleteFlag = true;
                cate.DeleteAt= DateTime.Now;
                RestaurantContext.Ins.Categories.Update(cate);
                RestaurantContext.Ins.SaveChanges();
            }
            return RedirectToPage("/Admin/category/cateManage");
        }
        public IActionResult OnPostActive()
        {
            string id = Request.Form["itemId"];
            var cate = RestaurantContext.Ins.Categories.Find(int.Parse(id));
            if (cate != null)
            {
                cate.DeleteFlag = false;
                cate.UpdateAt= DateTime.Now;
                cate.DeleteAt = null;
                RestaurantContext.Ins.Categories.Update(cate);
                RestaurantContext.Ins.SaveChanges();
            }
            return RedirectToPage("/Admin/category/cateManage");
        }

        public IActionResult OnPostUpdate()
        {
            string id = Request.Form["itemId"];
            string name = Request.Form["name"];
            var cate = RestaurantContext.Ins.Categories.Find(int.Parse(id));
            if (cate != null)
            {
                cate.Name=name;
                cate.UpdateAt = DateTime.Now;
                cate.DeleteAt=null;
                RestaurantContext.Ins.Categories.Update(cate);
                RestaurantContext.Ins.SaveChanges();
            }
            return RedirectToPage("/Admin/category/cateManage");
        }
    }
}
