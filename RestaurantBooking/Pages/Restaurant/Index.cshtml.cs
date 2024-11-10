using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        }

        public async Task<IActionResult> OnPostAsync()
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
                || startDate.CompareTo(DateTime.Now.AddMinutes(30)) <= 0)
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
            List<Table> table_List = await _context.Tables
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
             _context.Bookings.Add(bookInfo);
            await _context.SaveChangesAsync();

            //send mail confirm user
            SendMail(name, email, phone, date, time, table_List[0].Id.ToString()?? String.Empty);

            //real time
            _hub.Clients.All.SendAsync("LoadAll");
            return Page();
        }

        private static void SendMail(string name, string email, string phone, string date, string time, string tableName)
        {
            string body = BodyCFBooking(name, phone, date, time, tableName);
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            string emailFrom = config["InforMail:email"];
            string passFrom = config["InforMail:password"];
            Mail.instance.sendMail(emailFrom, passFrom, email, body, "Confirm Booking");
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

    }
}
