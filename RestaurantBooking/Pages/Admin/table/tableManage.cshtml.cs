using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantBooking.Pages.Admin.table
{
    public class tableManageModel : PageModel
    {
        public List<Table> tables { get; set; } = new List<Table>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        private void load(int pageIndex = 1, int pageSize = 5)
        {
            CurrentPage = pageIndex;
            var query = RestaurantContext.Ins.Tables.AsQueryable();
            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            tables = query
                        .OrderByDescending(x => x.Id)
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }
        public IActionResult OnGet( int pageIndex = 1, int pageSize = 5)
        {   
           load(pageIndex, pageSize);
            if (HttpContext.Session.GetString("role") == null ||
                HttpContext.Session.GetString("role") != "Admin") return Redirect("/Restaurant");
            return Page();
        }

        public IActionResult OnPostAdd()
        {   

            Table table = new Table();
            table.IsOrder = false;
            table.ForBooking = false;
            table.DeleteFlag = true;
            table.CreateAt = DateTime.Now;
            RestaurantContext.Ins.Tables.Add(table);
            RestaurantContext.Ins.SaveChanges();
            TempData["success"] = "Add Table successfull";
            return RedirectToPage("/Admin/table/tableManage");
        }

        public IActionResult OnPostDelete(int pageIndex = 1, int pageSize = 5)
        {
            string id = Request.Form["itemId"];
            var table = RestaurantContext.Ins.Tables.Find(int.Parse(id));
            if (table != null)
            {
                table.DeleteFlag = true;
                table.DeleteAt=DateTime.Now;
            }
            RestaurantContext.Ins.Tables.Update(table);
            RestaurantContext.Ins.SaveChanges();
            TempData["success"] = "Delete successfull";
            load(pageIndex, pageSize);
            return RedirectToPage("/Admin/table/tableManage");
        }

        public IActionResult OnPostActive(int pageIndex = 1, int pageSize = 5)
        {
            string id = Request.Form["itemId"];
            var table = RestaurantContext.Ins.Tables.Find(int.Parse(id));
            if (table != null)
            {
                table.DeleteFlag = false;
                table.UpdateAt=DateTime.Now;
            }
            RestaurantContext.Ins.Tables.Update(table);
            RestaurantContext.Ins.SaveChanges();
            TempData["success"] = "Active successfull";
            load(pageIndex, pageSize);
            return RedirectToPage("/Admin/table/tableManage");
        }

        public IActionResult OnPostUpdate(int pageIndex = 1, int pageSize = 5)
        {
            string id = Request.Form["itemId"];
            bool forBooking = Request.Form.ContainsKey("forBooking"); // Sẽ là true nếu checkbox được chọn

            var table = RestaurantContext.Ins.Tables.Find(int.Parse(id));
            if (table != null)
            {
                table.ForBooking = forBooking;
                table.UpdateAt = DateTime.Now;
                RestaurantContext.Ins.Tables.Update(table);
                RestaurantContext.Ins.SaveChanges();
            }

            TempData["success"] = "Update success";
            load(pageIndex, pageSize);
            return RedirectToPage("/Admin/table/tableManage");
        }

    }
}
