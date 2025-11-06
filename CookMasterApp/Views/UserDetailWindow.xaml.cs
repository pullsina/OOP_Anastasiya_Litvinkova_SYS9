using CookMasterApp.ViewModels;
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

namespace CookMasterApp.Views
{
    /// <summary>
    /// Interaction logic for UserDetailWindow.xaml
    /// </summary>
    public partial class UserDetailWindow : Window
    {
        public UserDetailWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_OnChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserDetailViewModel vm)
            {
                if (ReferenceEquals(sender, OldPasswordBox))
                    vm.OldPassword = ((PasswordBox)sender).Password;
                else if (ReferenceEquals(sender, NewPasswordBox))
                    vm.NewPassword = ((PasswordBox)sender).Password;
                else if (ReferenceEquals(sender, NewPasswordBox2))
                    vm.ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}