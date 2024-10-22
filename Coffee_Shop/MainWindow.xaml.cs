using BCrypt.Net;
using DataLibrary.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Coffee_Shop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUser.Text;
            string pass = txtPass.Password;
            if(string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Username or password null");
                return;
            }
            var acc = CoffeeShopContext.Ins.Accounts.Where(x => x.Username.Equals(user)).FirstOrDefault();
            if(acc == null)
            {
                MessageBox.Show("Username not existed");
                return;
            }
            bool password = BCrypt.Net.BCrypt.Verify(pass, acc.Password);
            if(!password)
            {
                MessageBox.Show("Password is not correct");
                return;
            }
            if (acc.IsActive==false)
            {
                MessageBox.Show("Account not active");
                ActiveAccount activeAccount = new ActiveAccount();
                activeAccount.Show();
                this.Hide();
            }
            MessageBox.Show("Login successfull");
        }

        private void hplSignup_Click(object sender, RoutedEventArgs e)
        {
            SignUp signUp=new SignUp();
            signUp.Show();
            this.Hide();
        }
    }
}