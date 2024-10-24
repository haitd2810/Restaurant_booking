using System.Net.Mail;
using System.Net;

namespace RestaurantBooking.Common
{
    public class Mail
    {
        public static Mail instance = new Mail();
        public Mail()
        {
            if (instance == null) instance = this;
        }
        public void sendMail(string from, string password, string to, string body, string subject)
        {
            string fromMail = from;
            string fromPass = password;
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = subject;
            message.To.Add(new MailAddress(to));
            message.Body = body;
            message.IsBodyHtml = true;
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPass),
                EnableSsl = true
            };
            smtpClient.Send(message);
        }
    }
}
