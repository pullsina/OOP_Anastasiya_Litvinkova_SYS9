using CookMasterApp.Managers;
using CookMasterApp.Views;
using MVVM_KlonaMIg.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CookMasterApp.ViewModels
{
    internal class RegisterViewModel : INotifyPropertyChanged
    {
        //PROPS
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        public string Message { get; set; }
        public List<String> Countries { get; } = new() { "Sweden", "Denmark", "Norway" };

        private string _selectedCountry;
        public string SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                _selectedCountry = value;
                OnPropertyChanged(nameof(SelectedCountry));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public List<string> SecurityQuestions { get; set; } = new()
        { "What is your favorite food?",
          "What is your mother's maiden name?",
          "What street did you grow up on?"
        };

        private string _selectedSecurityQuestion;
        public string SelectedSecurityQuestion
        {
            get { return _selectedSecurityQuestion; }
            set
            {
                _selectedSecurityQuestion = value;
                OnPropertyChanged(nameof(SelectedSecurityQuestion));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _securityAnswer;
        public string SecurityAnswer
        {
            get { return _securityAnswer; }
            set
            {
                _securityAnswer = value;
                OnPropertyChanged(nameof(SecurityAnswer));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand ReturnToMainCommand { get; }
        private readonly UserManager _userManager;

        //CONSTRUCTOR
        public RegisterViewModel()
        {
            _userManager = App.SharedUserManager;
            RegisterCommand = new RelayCommand(Register, CanRegister);
            ReturnToMainCommand = new RelayCommand(ReturnToMain);
        }
        public RegisterViewModel(UserManager sharedManager)
        {
            _userManager = sharedManager;
            RegisterCommand = new RelayCommand(Register, CanRegister);
            ReturnToMainCommand = new RelayCommand(ReturnToMain);
        }


        // METHODS
        private async void Register(object parameter)
        {
            string password = parameter is PasswordBox pb ? pb.Password : "";
            bool success = _userManager.Register(Username, password, SelectedCountry, SelectedSecurityQuestion, SecurityAnswer);
            if (success)
            {
                Message = "Registration successful!";
                OnPropertyChanged(nameof(Message));
                await Task.Delay(700); // låt användaren se meddelandet               

                var mainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(_userManager) // skicka in samma manager
                };
                mainWindow.Show();
                foreach (var w in Application.Current.Windows)
                {
                    if (w is RegisterWindow reg) reg.Close();
                }
            }
            else
            {
                Message = "Registration failed. Please check your input.";
                OnPropertyChanged(nameof(Message));
            }
        }

        private bool CanRegister(object parameter)
        {
            if (parameter is not PasswordBox passwordBox)
                return false;
            var mainPassword = passwordBox.Password;
            // Hämta PasswordBox2 från fönstret (lite överkurs)
            var registerWindow = Application.Current.Windows.OfType<RegisterWindow>().FirstOrDefault();
            var confirmPasswordBox = registerWindow?.FindName("PasswordBox2") as PasswordBox;
            var confirmPassword = confirmPasswordBox?.Password ?? "";

            return !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(SelectedCountry) &&
                !string.IsNullOrWhiteSpace(mainPassword) && 
                !string.IsNullOrWhiteSpace(confirmPassword) && 
                 mainPassword == confirmPassword &&
                !string.IsNullOrWhiteSpace(SelectedSecurityQuestion) &&
                !string.IsNullOrWhiteSpace(SecurityAnswer);
        }
        private void ReturnToMain(object parameter)
        {
            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel(_userManager) // skicka in samma manager
            };           
            mainWindow.Show();
            foreach (var w in Application.Current.Windows)
            {
                if (w is RegisterWindow reg) reg.Close();
            }
        }       

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
