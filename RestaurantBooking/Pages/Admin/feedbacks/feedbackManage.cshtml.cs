using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace RestaurantBooking.Pages.Admin.feedbacks
{
    public class feedbackManageModel : PageModel
    {
        public List<Feedback> feedbacks { get; set; } = new List<Feedback>();
        public List<Account> accounts { get; set; } = new List<Account>();
        public List<Role> roles { get; set; } = new List<Role>();
        public List<Category> categories { get; set; } = new List<Category>();
        public List<Menu> menus { get; set; } = new List<Menu>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string Employee { get; set; }
        public string Menu { get; set; }    
        public string Type { get; set; }
        public void OnGet(string employee,string type, string menu, int pageIndex = 1, int pageSize = 5)
        {
            Employee = employee;
            Menu = menu;
            Type = type;
            menus= RestaurantContext.Ins.Menus.ToList();
            accounts = RestaurantContext.Ins.Accounts.ToList();
            CurrentPage = pageIndex;
            var query = RestaurantContext.Ins.Feedbacks.AsQueryable();
            if(!string.IsNullOrEmpty(type) && !type.Equals("All"))
            {
                if (!string.IsNullOrEmpty(employee) && !employee.Equals("All"))
                {
                    query = query.Include(x => x.Account).Where(x => x.AccountId == int.Parse(employee) && x.Type.Equals(type));
                }
                if (!string.IsNullOrEmpty(menu) && !menu.Equals("All"))
                {
                    query = query.Where(x => x.MenuId == int.Parse(menu) && x.Type.Equals(type));
                }
            }
            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            feedbacks = query.Include(x => x.Account).Include(x => x.Menu)
            .OrderByDescending(i => i.Id)
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

        }
    }
}
