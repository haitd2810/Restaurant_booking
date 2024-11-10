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
        public string SearchRole { get; set; }
        public List<Account> acc { get; set; } = new List<Account>();
        public List<Role> roles { get; set; } = new List<Role>();

        public IActionResult OnGet(string search, string searchrole, int pageIndex = 1, int pageSize = 10)
        {
            if (HttpContext.Session.GetString("role") == null ||
                HttpContext.Session.GetString("role") != "Admin") return Redirect("/Restaurant");
            roles= RestaurantContext.Ins.Roles.ToList();
            Search = search;
            CurrentPage = pageIndex;
            SearchRole = searchrole;
            var query = RestaurantContext.Ins.Accounts.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Username.Contains(search));
            }
            if (!string.IsNullOrEmpty(searchrole))
            {
                if (!searchrole.Equals("All"))
                {
                    query = query.Where(x => x.RoleId == int.Parse(searchrole));
                }
            }
            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            acc = query.Include(x => x.Role)
            .OrderByDescending(i => i.Id)
            .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            return Page();
        }

        public IActionResult OnPostDelete()
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
            TempData["success"] = "Delete successful";
            return RedirectToPage("/Admin/accounts/accountManage");
        }
        public IActionResult OnPostActive()
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
            TempData["success"] = "Delete successful";
            return RedirectToPage("/Admin/accounts/accountManage");
        }

        public IActionResult OnPostUpdate()
        {
            string id = Request.Form["itemId"];
            string role = Request.Form["role"];
            var me = RestaurantContext.Ins.Accounts.Find(int.Parse(id));
            var r = RestaurantContext.Ins.Roles.Find(int.Parse(role));
            if (me != null)
            {
                me.RoleId = int.Parse(role);
                me.UpdateAt = DateTime.Now;
            }
            RestaurantContext.Ins.Accounts.Update(me);
            RestaurantContext.Ins.SaveChanges();
            if(r != null)
            {
                SendMailRole(me.Username, r.Name);
            }
            TempData["success"] = "Delete successful";
            return RedirectToPage("/Admin/accounts/accountManage");
        }

        public IActionResult OnPostReset()
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
            TempData["success"] = "Reset Password successful! Please check mail";
            return RedirectToPage("/Admin/accounts/accountManage");
        }

        private static void SendMailRole(string email, string role)
        {
            string body = $"Your role has been changed to {role} ";
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            string emailFrom = config["InforMail:email"];
            string passFrom = config["InforMail:password"];
            Mail.instance.sendMail(emailFrom, passFrom, email, body, "Change Role");
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
            Mail.instance.sendMail(emailFrom, passFrom, email, body, "Reset Password");
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

        public IActionResult OnPostAdd()
        {
            string name = Request.Form["user"];
            string role = Request.Form["role"];
            string pass = GenerateRandomString(6);
            if (string.IsNullOrEmpty(name))
            {
                TempData["error"] = "Email and password is not null";
                return RedirectToPage("/Admin/accounts/accountManage");
            }
            var acc = RestaurantContext.Ins.Accounts.Where(x => x.Username.Equals(name)).FirstOrDefault();
            if (acc != null)
            {
                TempData["error"] = "Email existed";
                return RedirectToPage("/Admin/accounts/accountManage");
            }
            string hash = BCrypt.Net.BCrypt.HashPassword(pass);
            var account = new Account();
            account.Username = name;
            account.Password = hash;
            account.CreateAt = DateTime.Now;
            account.IsActive = false;
            account.RoleId = int.Parse(role);
            if (int.Parse(role) == 1)
            {
                account.Code = "S" + GenerateRandomInt(4);
            }else if(int.Parse(role) == 2)
            {
                account.Code = "C" + GenerateRandomInt(4);
            }
            RestaurantContext.Ins.Accounts.Add(account);
            RestaurantContext.Ins.SaveChanges();
            var a = RestaurantContext.Ins.Accounts.Where(x => x.Username.Equals(name)).FirstOrDefault();
            if (a == null)
            {
                return RedirectToPage("/Admin/accounts/accountManage");
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
            TempData["success"] = "Add account successful";
            SendMail(name, token,pass);
            return RedirectToPage("/Admin/accounts/accountManage");
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GenerateRandomInt(int length)
        {
            const string chars = "0123456789";
            var random = new Random();
            string s = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            var acc = RestaurantContext.Ins.Accounts.Where(x => x.Code.Equals(s)).FirstOrDefault();
            if(acc != null)
            {
                return GenerateRandomInt(4);
            }
            return s;
        }
    }
}
