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
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
        }
        private void PasswordBox_OnChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ForgotPasswordViewModel vm)
            {
                if (ReferenceEquals(sender, PasswordBox))
                    vm.Password = PasswordBox.Password;
                else if (ReferenceEquals(sender, PasswordBox2))
                    vm.ConfirmPassword = PasswordBox2.Password;
            }

            // Uppdaterar Command-systemet så att CanExecute() körs igen
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
