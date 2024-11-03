using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Common;

namespace RestaurantBooking.Pages.Admin.accounts
{
    public class accountManageModel : PageModel
    {
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string Search { get; set; }
        public List<Account> acc { get; set; } = new List<Account>();
        private IActionResult load(string search, int pageIndex = 1, int pageSize = 10)
        {
            Search = search;
            CurrentPage = pageIndex;

            var query = RestaurantContext.Ins.Accounts.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Username.Contains(search));
            }
            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            acc = query. Include(x => x.Role)
            .OrderByDescending(i => i.Id)
            .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            return Page();
        }

        public void OnGet(string search, int pageIndex = 1, int pageSize = 10)
        {
            load(search, pageIndex, pageSize);
        }

        public IActionResult OnPostDelete(string search, int pageIndex = 1, int pageSize = 10)
        {
            string id = Request.Form["itemId"];
            var me = RestaurantContext.Ins.Accounts.Find(int.Parse(id));
            if (me != null)
            {
                me.IsActive = false;
                me.DeleteAt = DateTime.Now;
            }
            RestaurantContext.Ins.Accounts.Update(me);
            RestaurantContext.Ins.SaveChanges();
            ViewData["success"] = "Delete successful";
            load(search, pageIndex, pageSize);
            return Page();
        }
        public IActionResult OnPostActive(string search, int pageIndex = 1, int pageSize = 10)
        {
            string id = Request.Form["itemId"];
            var me = RestaurantContext.Ins.Accounts.Find(int.Parse(id));
            if (me != null)
            {
                me.IsActive = true;
                me.UpdateAt = DateTime.Now;
            }
            RestaurantContext.Ins.Accounts.Update(me);
            RestaurantContext.Ins.SaveChanges();
            ViewData["success"] = "Delete successful";
            load(search, pageIndex, pageSize);
            return Page();
        }

        public IActionResult OnPostReset(string search, int pageIndex = 1, int pageSize = 10)
        {
            string id = Request.Form["itemId"];
            var me = RestaurantContext.Ins.Accounts.Find(int.Parse(id));
            string pass = GenerateRandomString(6);
            string hash = BCrypt.Net.BCrypt.HashPassword(pass);
            if (me != null)
            {
                me.Password=hash;
                me.UpdateAt = DateTime.Now;
               
            }
            RestaurantContext.Ins.Accounts.Update(me);
            RestaurantContext.Ins.SaveChanges();
            SendMailReset(me.Username, pass);
            ViewData["success"] = "Reset Password successful! Please check mail";
            load(search, pageIndex, pageSize);
            return Page();
        }

        private static void SendMailReset(string email, string pass)
        {
            string body = $"Your password is {pass}";
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            string emailFrom = config["InforMail:email"];
            string passFrom = config["InforMail:password"];
            Mail.instance.sendMail(emailFrom, passFrom, email, body, "Active Account");
        }

        private static void SendMail(string email, string token, string pass)
        {
            string url = $"https://localhost:7144/active?token={token}";
            string body = $"Hello {email},\n\nYour activation {url}. Please enter this code in the application to activate your account \n\n Your password is: {pass}";
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            string emailFrom = config["InforMail:email"];
            string passFrom = config["InforMail:password"];
            Mail.instance.sendMail(emailFrom, passFrom, email, body, "Active Account");
        }

        public IActionResult OnPostAdd(string search, int pageIndex = 1, int pageSize = 10)
        {
            string name = Request.Form["user"];
            string pass = GenerateRandomString(6);
            if (string.IsNullOrEmpty(name))
            {
                ViewData["error"] = "Email and password is not null";
                load(search, pageIndex, pageSize);
                return Page();
            }
            var acc = RestaurantContext.Ins.Accounts.Where(x => x.Username.Equals(name)).FirstOrDefault();
            if (acc != null)
            {
                ViewData["error"] = "Email existed";
                load(search, pageIndex, pageSize);
                return Page();
            }
            string hash = BCrypt.Net.BCrypt.HashPassword(pass);
            var account = new Account
            {
                Username = name,
                Password = hash,
                RoleId = 1,
                CreateAt = DateTime.Now,
                IsActive = true
            };
            RestaurantContext.Ins.Accounts.Add(account);
            RestaurantContext.Ins.SaveChanges();
            var a = RestaurantContext.Ins.Accounts.Where(x => x.Username.Equals(name)).FirstOrDefault();
            if (a == null)
            {
                load(search, pageIndex, pageSize);
                return Page();
            }
            string token = Guid.NewGuid().ToString();
            Token to = new Token
            {
                Token1 = token,
                AccountId = a.Id,
                CreateAt = DateTime.Now
            };
            HttpContext.Session.SetInt32("accId", a.Id);
            RestaurantContext.Ins.Tokens.Add(to);
            RestaurantContext.Ins.SaveChanges();
            ViewData["success"] = "Add account successful";
            SendMail(name, token,pass);
            load(search, pageIndex, pageSize);
            return Page();
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
