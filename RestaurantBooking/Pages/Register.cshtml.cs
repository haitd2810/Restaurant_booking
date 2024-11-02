using BCrypt.Net;
using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Net;
using RestaurantBooking.Common;

namespace RestaurantBooking.Pages
{
    public class RegisterModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
           
            //RestaurantContext.Ins.Tokens.Add(token);
            string name = Request.Form["name"];
            string password = Request.Form["password"];
            string comfirm = Request.Form["comfirm"];
            if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
            {
				ViewData["error"] = "Email and password is not null";
				return Page();
			}
            var acc = RestaurantContext.Ins.Accounts.Where(x => x.Username.Equals(name)).FirstOrDefault();
            if(acc != null)
            {
                ViewData["errorE"] = "Email existed";
                return Page();
            }
            if (!password.Equals(comfirm))
            {
				ViewData["errorC"] = "Comfirm password not correct";
				return Page();
			}
            string hash= BCrypt.Net.BCrypt.HashPassword(password);
            var account = new Account
            {
                Username = name,
                Password = hash,
                RoleId = 1,
                CreateAt=DateTime.Now,
                IsActive = true
            };
            RestaurantContext.Ins.Accounts.Add(account);
            RestaurantContext.Ins.SaveChanges();
            var a = RestaurantContext.Ins.Accounts.Where(x => x.Username.Equals(name)).FirstOrDefault();
            if(a == null)
            {
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
            SendMail(name,token);
            return Page();
        }
        private static void SendMail(string email, string token)
        {
            string url = $"https://localhost:7144/active?token={token}";
            string body = $"Hello {email},\n\nYour activation {url}. Please enter this code in the application to activate your account";
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            string emailFrom = config["InforMail:email"];
            string passFrom = config["InforMail:password"];
            Mail.instance.sendMail(emailFrom, passFrom, email, body, "Acrive Account");
        }
	}
}
