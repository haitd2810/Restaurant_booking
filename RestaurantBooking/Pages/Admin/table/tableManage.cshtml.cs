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
                        .OrderByDescending(i => i.Id)
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }
        public void OnGet( int pageIndex = 1, int pageSize = 5)
        {
           load(pageIndex, pageSize);
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
            ViewData["success"] = "Add Table successfull";
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
            ViewData["success"] = "Delete successfull";
            load(pageIndex, pageSize);
            return Page();
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
            ViewData["success"] = "Active successfull";
            load(pageIndex, pageSize);
            return Page();
        }

        public IActionResult OnPostUpdate( int pageIndex = 1, int pageSize = 5)
        {
            string id = Request.Form["itemId"];
            bool forBooking = Request.Form["forBooking"] == "true";
            var table = RestaurantContext.Ins.Tables.Find(int.Parse(id));
            if (table != null)
            {
                table.ForBooking = forBooking;
                table.UpdateAt = DateTime.Now;
                RestaurantContext.Ins.Tables.Update(table);
                RestaurantContext.Ins.SaveChanges();
            }
            ViewData["success"] = "Update success";
            load( pageIndex, pageSize);
            return Page(); ;
        }
    }
}
