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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // Hanterar händelsen PasswordChanged för PasswordBox i MainWindow.
        // Eftersom PasswordBox.Password inte kan bindas direkt till ViewModel,
        // uppdateras lösenordet manuellt i MainViewModel varje gång användaren skriver något.
        // CommandManager.InvalidateRequerySuggested() anropas för att aktivera/deaktivera knappar 
        // som är beroende av om lösenordet är ifyllt (t.ex. LoginCommand).
        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.MainViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
                CommandManager.InvalidateRequerySuggested();
            }
        }

    }
}
