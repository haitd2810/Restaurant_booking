using BCrypt.Net;
using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Coffee_Shop
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        
        public SignUp()
        {
            InitializeComponent();
        }

        private void btnSignup_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;
            string password = txtPass.Password;
            string comfirm = txtComfirm.Password;
            if(string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username and password null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(!Regex.IsMatch(user, "^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+$\r\n"))
            {
                MessageBox.Show("Invalid email", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!password.Equals(comfirm))
            {
                MessageBox.Show("Comfirm password is not correct", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var accs = RestaurantContext.Ins.Accounts.Where(x => x.Username.Equals(user)).FirstOrDefault();
            if(accs != null)
            {
                MessageBox.Show("Username existed");
                return;
            }
            string hashpass= BCrypt.Net.BCrypt.HashPassword(password);
            var acc = new Account()
            {
                Username = user,
                Password = hashpass,
                RoleId = 1,
                IsActive = false
            };
            RestaurantContext.Ins.Accounts.Add(acc);
            RestaurantContext.Ins.SaveChanges();
            SendConfirmationEmail(user);
            MessageBox.Show($"Registered successfully. Please go to {user} to activate your account", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            ActiveAccount activeAccount = new ActiveAccount()
            {
                acc = acc
            };
            //MainWindow mainWindow =new MainWindow();
            activeAccount.Show();
            this.Hide();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void hplLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(); 
            main.Show();
            this.Hide();
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
                Date = DateTime.Now,
                AccountId=acc.Id
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

        //private bool ValidateToken(string email, string token)
        //{
        //    // Lấy thời gian hiện tại
        //    DateTime now = DateTime.UtcNow;

        //    // Duyệt qua 10 phút gần nhất
        //    for (int i = 0; i <= 10; i++)
        //    {
        //        // Tạo lại token từ email và thời gian trong vòng 10 phút
        //        string timestamp = now.AddMinutes(-i).ToString("yyyyMMddHHmmss");
        //        string expectedToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(email + timestamp));

        //        // Kiểm tra nếu token khớp
        //        if (token == expectedToken)
        //        {
        //            return true; // Token hợp lệ
        //        }
        //    }

        //    return false; // Token không hợp lệ
        //}

        //public void ActivateAccount(string email, string token)
        //{
        //    if (ValidateToken(email, token))
        //    {
        //        // Nếu token hợp lệ, kích hoạt tài khoản
        //        var acc = RestaurantContext.Ins.Accounts.FirstOrDefault(x => x.Username == email);
        //        if (acc != null)
        //        {
        //            acc.IsActive = true;
        //            RestaurantContext.Ins.SaveChanges();

        //            MessageBox.Show("Account successfully activated!");
        //        }
        //    }
        //    else
        //    {
        //        // Token không hợp lệ
        //        MessageBox.Show("Invalid or expired activation link.");
        //    }
        //}

    }
}
