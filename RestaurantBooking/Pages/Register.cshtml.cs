using BCrypt.Net;
using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Net;

namespace RestaurantBooking.Pages
{
    public class RegisterModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {   
            string name= Request.Form["name"];
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
                IsActive = false
            };
            RestaurantContext.Ins.Accounts.Add(account);
            RestaurantContext.Ins.SaveChanges();
            return Page();
        }

		private void SendConfirmationEmail(string email)
		{
			Random random = new Random();
			string randomNumbers = "";

			for (int i = 0; i < 6; i++)
			{
				int num = random.Next(0, 10);
				randomNumbers += num.ToString();
			}
			string token = randomNumbers;
			var acc = RestaurantContext.Ins.Accounts.Where(x => x.Username.Equals(email)).FirstOrDefault();
			Token t = new Token()
			{
				Token1 = token,
				CreateAt = DateTime.Now,
				AccountId = acc.Id
			};
			RestaurantContext.Ins.Tokens.Add(t);
			RestaurantContext.Ins.SaveChanges();
			string mail = "phanquan2092003@gmail.com";
			string password = "pndc pnkc egon mxpe";
			SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
			{
				Credentials = new NetworkCredential(mail, password),
				EnableSsl = true
			};
			string Subject = "Account Activation";
			string Body = $"Hello {email},\n\nYour activation code is: {token}. Please enter this code in the application to activate your account";

			smtpClient.Send(mail, email, Subject, Body);
		}

	}
}
