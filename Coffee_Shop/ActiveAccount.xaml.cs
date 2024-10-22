using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Coffee_Shop
{
    /// <summary>
    /// Interaction logic for ActiveAccount.xaml
    /// </summary>
    public partial class ActiveAccount : Window
    {
        public Account acc { get; internal set; }

        public ActiveAccount()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnActive_Click(object sender, RoutedEventArgs e)
        {
            string code = txtCode.Text;
            string email = txtEmail.Text;
            var a = RestaurantContext.Ins.Accounts.Where(x => x.Username.Equals(email)).FirstOrDefault();
            if (a == null)
            {
                MessageBox.Show("Account not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var token = RestaurantContext.Ins.Tokens
                .Where(x => x.Account.Id == a.Id)
                .OrderByDescending(x => x.Date) 
                .FirstOrDefault();
            if (code != token.Token1)
            {
                MessageBox.Show("Code not correct", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            a.IsActive = true;
            RestaurantContext.Ins.Accounts.Update(a);
            RestaurantContext.Ins.SaveChanges();
            MessageBox.Show("Active account successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }
    }
}
