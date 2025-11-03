using System.Windows;
using System.Windows.Controls;
using CookMasterApp.ViewModels;

namespace CookMasterApp.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_OnChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                // Kolla vilket fält som ändrades
                if (sender is PasswordBox pb)
                {
                    if (pb.Name == "PasswordBox")
                        vm.Password = pb.Password; // uppdatera huvudlösenord
                    else if (pb.Name == "PasswordBox2")
                        vm.ConfirmPassword = pb.Password; // uppdatera bekräftelse
                }
            }
        }
    }
}
