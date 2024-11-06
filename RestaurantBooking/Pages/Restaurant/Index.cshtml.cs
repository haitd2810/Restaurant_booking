using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RestaurantBooking.Common;
using RestaurantBooking.Hubs;
using System;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CoffeeShopCustomer.Pages.CoffeePage
{
    public class IndexModel : PageModel
    {
        public List<Menu> menu_list { get; set; }
        public List<Category> category_list { get; set; }

        public List<Account> accounts_list { get; set; }

        private Regex regexPhone = new Regex("^[0-9]+$");
        private Regex regexName = new Regex("^[a-zA-Z ]+$");
        private readonly IHubContext<HubServer> _hub;
        private readonly RestaurantContext _context = new RestaurantContext();
        public IndexModel(IHubContext<HubServer> hub, RestaurantContext context)
        {
            this._hub = hub;
            _context = context;
        }
        public async Task OnGetAsync()
        {
            category_list = await _context.Categories.Where(ct => ct.DeleteFlag == false).ToListAsync();
            menu_list = await _context.Menus.Include(m => m.Cate).Where(m => m.DeleteFlag == false).ToListAsync();
            accounts_list = await _context.Accounts.Where(a => a.IsActive == true).ToListAsync();
        }
        public string emailBook { get; set; } = "";
        public async Task<IActionResult> OnPostBookingAsync()
        {
            //validate phone
            string phone = Request.Form["phone"];
            if (!regexPhone.IsMatch(phone))
            {
                HttpContext.Session.SetString("Booking_Failed", "Invalid Phone Request");
                return Redirect("/Restaurant");
            }

            //validate date
            string date = Request.Form["date"];
            string time = Request.Form["time"];
            DateTime startDate = DateTime.Parse($"{date} {time}");
            if (startDate.CompareTo(DateTime.Now.AddDays(1)) > 0
                || startDate.CompareTo(DateTime.Now.AddMinutes(30)) <= 0 || startDate.Hour < 7 || startDate.Hour > 23)
            {
                HttpContext.Session.SetString("Booking_Failed", "Invalid Date Time Request");
                return Redirect("/Restaurant");
            }

            //check if this time have full table to order
            List<Booking> booked_List = await _context.Bookings
                .Where(b => b.Status == "booked"
                &&  b.StartDate <= startDate.AddHours(1)
                && b.StartDate >= startDate.AddHours(-1)
                && b.StartDate.Value.AddMinutes(15).CompareTo(DateTime.Now) > 0)
                .ToListAsync();
            var bookedTableIds = booked_List.Select(b => b.TableId).ToList();
            //check if there are any table is free
            List<DataLibrary.Models.Table> table_List = await _context.Tables
                .Where(t => t.DeleteFlag == false && !bookedTableIds.Contains(t.Id) && t.ForBooking == true)
                .ToListAsync();
            if (table_List.Count == 0)
            {
                HttpContext.Session.SetString("Booking_Failed", "Table is full at this time");
                return Redirect("/Restaurant");
            }
            
            //check name is valid in alphabet
            string name = Request.Form["name"];
            if (!regexName.IsMatch(name)) {
                HttpContext.Session.SetString("Booking_Failed", "Invalid Name Request (Name Just contains character in Alphabet)");
                return Redirect("/Restaurant");
            }
            //remove session if all condition ok
            HttpContext.Session.Remove("Booking_Failed");

            
            //insert db
            string email = Request.Form["email"];
            string status = "booked";
            int tableId = table_List[0].Id;
            Booking bookInfo = new Booking()
            {
                FullName = name,
                Email = email,
                Phone = phone,
                StartDate = startDate,
                Status = status,
                TableId = tableId
            };
            List<Booking> bookList = await _context.Bookings.Where(b => b.Email == email 
                                                                    && b.Phone == phone 
                                                                    && b.Status == "confirm"
                                                                    && b.StartDate.Value.Date.CompareTo(DateTime.Now.Date.AddMonths(-1)) >= 0).ToListAsync();
            if (bookList.Count == 0) bookInfo.NumberOfBooking = 1;
            else bookInfo.NumberOfBooking = bookList.Count + 1;
             _context.Bookings.Add(bookInfo);
            await _context.SaveChangesAsync();

            //send mail confirm user
            string body = BodyCFBooking(name, phone, date, time, table_List[0].Id.ToString() ?? String.Empty);
            SendMail(body, "Confirm Booking", email);
            //real time
            _hub.Clients.All.SendAsync("LoadAll");
            return Page();
        }

        public async Task<IActionResult> OnPostFeedbackAsync(string phone, string email, 
            string date, IFormFile img, string type, string feedback, string staff, string meal)
        {
            if (!regexPhone.IsMatch(phone))
            {
                HttpContext.Session.SetString("feedback_Failed", "Invalid Phone Feedback");
                return Redirect("/Restaurant");
            }
            if(DateTime.Parse(date).Date.CompareTo(DateTime.Now.Date) > 0)
            {
                HttpContext.Session.SetString("feedback_Failed", "Invalid Date Feedback");
                return Redirect("/Restaurant");
            }
            Booking book = await _context.Bookings.Where(b => b.Email == email
                                                        && b.Phone == phone
                                                        && b.Status == "confirm"
                                                        && b.StartDate.HasValue
                                                        && b.StartDate.Value.Date.CompareTo(DateTime.Parse(date).Date) == 0).FirstOrDefaultAsync();
            if (book == null) {
                HttpContext.Session.SetString("feedback_Failed", "Cannot find your information booking in this system!");
                return Redirect("/Restaurant");
            }

            HttpContext.Session.Remove("feedback_Failed");
            Feedback fb = new Feedback()
            {
                Phone = phone,
                Email = email,
                Feedback1 = feedback,
                Img = "wwwroot/assets/img/feedback/" + img.FileName,
                CreateAt = DateTime.Now,
                FeedbackForDate = DateTime.Parse(date),
            };
            if (type == "account") fb.AccountId = int.Parse(staff);
            else if (type == "menu") fb.MenuId = int.Parse(meal);
            else fb.Type = "other";

            _context.Feedbacks.Add(fb);
            int row = await _context.SaveChangesAsync();
            if(row > 0)
            {
                var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(img.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                {
                    HttpContext.Session.SetString("feedback_Failed", " file must be an image (.jpg, .jpeg, .png, .gif, .bmp).");
                    return Page();
                }

                var filePath = Path.Combine("wwwroot/assets/img/feedback", img.FileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                string body = BodyCFFeedback();
                SendMail(body, "Confirm Feedback", email);
            }
            return Redirect("/Restaurant");
        }

        private static void SendMail(string body, string title, string mail)
        {
            
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            string emailFrom = config["InforMail:email"];
            string passFrom = config["InforMail:password"];
            Mail.instance.sendMail(emailFrom, passFrom, mail, body, title);
        }

        private static string BodyCFBooking(string name, string phone, string date, string time, string tableNumber)
        {
            string body = "<!DOCTYPE html>" +
                "\r\n<html>\r\n" +
                "<head>\r\n" +
                "<style>\r\n" +
                "body {\r\n" +
                "font-family: Arial, sans-serif;\r\n" +
                "margin: 0;\r\n" +
                "padding: 0;\r\n" +
                "background-color: #f6f6f6;\r\n" +
                "}\r\n" +
                ".container {\r\n" +
                "width: 100%;\r\n" +
                "padding: 20px;\r\n" +
                "background-color: #ffffff;\r\n" +
                "box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);\r\n" +
                "margin: 20px auto;\r\n" +
                "max-width: 600px;\r\n" +
                "}\r\n" +
                ".header {\r\n" +
                "background-color: #333333;\r\n" +
                "color: #ffffff;\r\n" +
                "padding: 10px 20px;\r\n" +
                "text-align: center;\r\n" +
                "}\r\n" +
                ".content {\r\n" +
                "padding: 20px;\r\n" +
                "}\r\n" +
                ".content h2 {\r\n" +
                "color: #333333;\r\n" +
                "}\r\n" +
                ".content p {\r\n" +
                "line-height: 1.6;\r\n" +
                "color: #666666;\r\n" +
                "}\r\n" +
                ".content a {\r\n" +
                "display: inline-block;\r\n" +
                "padding: 10px 20px;\r\n" +
                "color: #ffffff;\r\n" +
                "background-color: #007bff;\r\n" +
                "text-decoration: none;\r\n" +
                "border-radius: 5px;\r\n" +
                "}\r\n" +
                ".footer {\r\n" +
                "background-color: #f1f1f1;\r\n" +
                "padding: 10px 20px;\r\n" +
                "text-align: center;\r\n" +
                "font-size: 12px;\r\n" +
                "color: #666666;\r\n" +
                "}\r\n" +
                "</style>\r\n" +
                "</head>\r\n" +
                "<body>\r\n" +
                "<div class=\"container\">\r\n" +
                "<div class=\"header\">\r\n" +
                "<h1>Restaurantly</h1>\r\n" +
                "</div>\r\n" +
                "<div class=\"content\">\r\n" +
                "<h2>Table Booking Confirmation</h2>\r\n" +
                "<p>Dear " + name + ",</p>\r\n" +
                "<p>Thank you for choosing our restaurant. Your table booking has been successfully confirmed.</p>\r\n" +
                "<p><strong>Booking Details:</strong></p>\r\n" +
                "<ul>\r\n" +
                "<li><strong>Booking Number:</strong> " + phone + "</li>\r\n" +
                "<li><strong>Date:</strong> " + date + "</li>\r\n" +
                "<li><strong>Time:</strong> " + time + "</li>\r\n" +
                "<li><strong>Table Number:</strong> " + tableNumber + "</li>\r\n" +
                "</ul>\r\n" +
                "<p>Kindly be advised that if you are more than 15 minutes late, your reservation will be canceled. Thank you for your understanding and cooperation.</p>\r\n" +
                "<p>We look forward to serving you and making your dining experience memorable. If you have any questions or need to make any changes to your booking, please do not hesitate to contact us.</p>\r\n" +
                "<p>Best regards,</p>\r\n" +
                "<p>Restaurantly Hari</p>\r\n" +
                "<a href=\"https://localhost:7148/\">Visit Our Website</a>\r\n" +
                "</div>\r\n" +
                "<div class=\"footer\">\r\n" +
                "<p>&copy; 2024 [Restaurant Name]. All rights reserved.</p>\r\n" +
                "<p>[Restaurant Address] | [Restaurant Phone Number] | [Restaurant Email]</p>\r\n" +
                "</div>\r\n" +
                "</div>\r\n" +
                "</body>\r\n" +
                "</html>";

            return body;
        }

        private static string BodyCFFeedback()
        {
            string body = "<!DOCTYPE html>" +
                "\r\n<html>\r\n" +
                "<head>\r\n" +
                "<style>\r\n" +
                "body {\r\n" +
                "font-family: Arial, sans-serif;\r\n" +
                "margin: 0;\r\n" +
                "padding: 0;\r\n" +
                "background-color: #f6f6f6;\r\n" +
                "}\r\n" +
                ".container {\r\n" +
                "width: 100%;\r\n" +
                "padding: 20px;\r\n" +
                "background-color: #ffffff;\r\n" +
                "box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);\r\n" +
                "margin: 20px auto;\r\n" +
                "max-width: 600px;\r\n" +
                "}\r\n" +
                ".header {\r\n" +
                "background-color: #333333;\r\n" +
                "color: #ffffff;\r\n" +
                "padding: 10px 20px;\r\n" +
                "text-align: center;\r\n" +
                "}\r\n" +
                ".content {\r\n" +
                "padding: 20px;\r\n" +
                "}\r\n" +
                ".content h2 {\r\n" +
                "color: #333333;\r\n" +
                "}\r\n" +
                ".content p {\r\n" +
                "line-height: 1.6;\r\n" +
                "color: #666666;\r\n" +
                "}\r\n" +
                ".content a {\r\n" +
                "display: inline-block;\r\n" +
                "padding: 10px 20px;\r\n" +
                "color: #ffffff;\r\n" +
                "background-color: #007bff;\r\n" +
                "text-decoration: none;\r\n" +
                "border-radius: 5px;\r\n" +
                "}\r\n" +
                ".footer {\r\n" +
                "background-color: #f1f1f1;\r\n" +
                "padding: 10px 20px;\r\n" +
                "text-align: center;\r\n" +
                "font-size: 12px;\r\n" +
                "color: #666666;\r\n" +
                "}\r\n" +
                "</style>\r\n" +
                "</head>\r\n" +
                "<body>\r\n" +
                "<div class=\"container\">\r\n" +
                "<div class=\"header\">\r\n" +
                "<h1>Restaurantly</h1>\r\n" +
                "</div>\r\n" +
                "<div class=\"content\">\r\n" +
                "<h2>Feedback Confirmation</h2>\r\n" +
                "<p>Thank you for taking the time to review our restaurant. Your review will be noted and improved by us.</p>\r\n" +
                "<p>If there are any other problems, do not hesitate to contact us immediately.</p>\r\n" +
                "<p>Thank you very much</p>\r\n" +
                "<p>Best regards,</p>\r\n" +
                "<p>Restaurantly Hari</p>\r\n" +
                "<a href=\"https://localhost:7148/\">Visit Our Website</a>\r\n" +
                "</div>\r\n" +
                "<div class=\"footer\">\r\n" +
                "<p>&copy; 2024 [Restaurant Name]. All rights reserved.</p>\r\n" +
                "<p>[Restaurant Address] | [Restaurant Phone Number] | [Restaurant Email]</p>\r\n" +
                "</div>\r\n" +
                "</div>\r\n" +
                "</body>\r\n" +
                "</html>";

            return body;
        }

    }
}
