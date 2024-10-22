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
        public string token { get; internal set; }
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
            //string code =txtCode.Text;
            //string email=txtEmail.Text;
            //var a = CoffeeShopContext.Ins.Accounts.Where(x => x.Username.Equals(email)).FirstOrDefault();
            //if(a == null)
            //{
            //    MessageBox.Show("Account not exist");
            //    return;
            //}
            //if(code != token)
            //{
            //    MessageBox.Show("Code not correct");
            //    return;
            //}
            //a.IsActive = true;
            //CoffeeShopContext.Ins.Accounts.Update(a);
            //CoffeeShopContext.Ins.SaveChanges();
            //MainWindow mainWindow = new MainWindow();
            //mainWindow.Show();
            //this.Hide();
        }
    }
}
