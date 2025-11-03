using CookMasterApp.Managers;
using CookMasterApp.Views;
using MVVM_KlonaMIg.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CookMasterApp.ViewModels
{
    internal class RegisterViewModel : INotifyPropertyChanged
    {
        // -------------------------
        // PROPERTIES
        // -------------------------

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                ValidateUsername();
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _selectedCountry;
        public string SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _selectedSecurityQuestion;
        public string SelectedSecurityQuestion
        {
            get => _selectedSecurityQuestion;
            set
            {
                _selectedSecurityQuestion = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _securityAnswer;
        public string SecurityAnswer
        {
            get => _securityAnswer;
            set
            {
                _securityAnswer = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public List<string> Countries { get; } = new() { "Sweden", "Denmark", "Norway" };
        public List<string> SecurityQuestions { get; } = new()
        {
            "What is your favorite food?",
            "What is your mother's maiden name?",
            "What street did you grow up on?"
        };

        // -------------------------
        // VALIDATION & MESSAGES
        // -------------------------

        public string Message { get; set; }
        public string MessageColor { get; set; } = "Red"; // Default red (error)
        public string UsernameMessage { get; set; }
        public string PasswordMessage { get; set; }
        public string ConfirmPasswordMessage { get; set; }
        private string _confirmPasswordColor = "Black";
        public string ConfirmPasswordColor
        {
            get => _confirmPasswordColor;
            set
            {
                _confirmPasswordColor = value;
                OnPropertyChanged();
            }
        }


        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                ValidatePassword();
                ValidatePasswordsMatch();
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                ValidatePasswordsMatch();
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private readonly UserManager _userManager;
        public ICommand RegisterCommand { get; }
        public ICommand ReturnToMainCommand { get; }

        // -------------------------
        // CONSTRUCTORS
        // -------------------------

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

        // -------------------------
        // VALIDATION METHODS
        // -------------------------

        private void ValidateUsername()
        {
            if (string.IsNullOrWhiteSpace(Username))
                UsernameMessage = "Username cannot be empty.";
            else if (Username.Length < 3)
                UsernameMessage = "Username must be at least 3 characters.";
            else if (_userManager.FindUser(Username) != null)
                UsernameMessage = "This username is already taken.";
            else
                UsernameMessage = "";

            OnPropertyChanged(nameof(UsernameMessage));
        }

        private void ValidatePassword()
        {
            var (isValid, message) = UserManager.IsPasswordValid(Password);
            PasswordMessage = isValid ? "" : message;
            OnPropertyChanged(nameof(PasswordMessage));
        }

        private void ValidatePasswordsMatch()
        {
            if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                ConfirmPasswordMessage = "";
                ConfirmPasswordColor = "Black";
            }
            else if (Password == ConfirmPassword)
            {
                ConfirmPasswordMessage = "Passwords match ✅";
                ConfirmPasswordColor = "DarkGreen";
            }
            else
            {
                ConfirmPasswordMessage = "Passwords do not match ❌";
                ConfirmPasswordColor = "Red";
            }

            OnPropertyChanged(nameof(ConfirmPasswordMessage));
            OnPropertyChanged(nameof(ConfirmPasswordColor));
        }


        // -------------------------
        // COMMAND METHODS
        // -------------------------

        private async void Register(object parameter)
        {
            string password = parameter is PasswordBox pb ? pb.Password : "";

            var (success, message) = _userManager.Register(Username, password, SelectedCountry, SelectedSecurityQuestion, SecurityAnswer);

            Message = message;
            MessageColor = success ? "Green" : "Red";
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(MessageColor));

            if (success)
            {
                await Task.Delay(700); // låt användaren se "Registration successful!"
                var mainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(_userManager)
                };
                mainWindow.Show();

                foreach (var w in Application.Current.Windows)
                {
                    if (w is RegisterWindow reg) reg.Close();
                }
            }
        }

        private bool CanRegister(object parameter)
        {
            if (parameter is not PasswordBox passwordBox)
                return false;
            var mainPassword = passwordBox.Password;

            var registerWindow = Application.Current.Windows.OfType<RegisterWindow>().FirstOrDefault();
            var confirmPasswordBox = registerWindow?.FindName("PasswordBox2") as PasswordBox;
            var confirmPassword = confirmPasswordBox?.Password ?? "";

            return !string.IsNullOrWhiteSpace(Username)
                && string.IsNullOrEmpty(UsernameMessage)
                && string.IsNullOrEmpty(PasswordMessage)
                && mainPassword == confirmPassword
                && !string.IsNullOrWhiteSpace(SelectedCountry)
                && !string.IsNullOrWhiteSpace(SelectedSecurityQuestion)
                && !string.IsNullOrWhiteSpace(SecurityAnswer);
        }

        private void ReturnToMain(object parameter)
        {
            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel(_userManager)
            };
            mainWindow.Show();

            foreach (var w in Application.Current.Windows)
            {
                if (w is RegisterWindow reg) reg.Close();
            }
        }

        // -------------------------
        // INotifyPropertyChanged
        // -------------------------

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
