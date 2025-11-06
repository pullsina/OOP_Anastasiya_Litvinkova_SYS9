using CookMasterApp.Managers;
using CookMasterApp.Views;
using MVVM_KlonaMIg.MVVM;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CookMasterApp.ViewModels
{
    internal class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        private readonly UserManager _userManager;

        public ForgotPasswordViewModel()
        {
            _userManager = App.SharedUserManager;
            ResetPasswordCommand = new RelayCommand(ResetPassword, CanResetPassword);
            ReturnToMainCommand = new RelayCommand(ReturnToMain);
        }

        // ---------------- INPUTS ----------------
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                LoadSecurityQuestion();
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

        public string SecurityQuestion { get; private set; } = "";

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

        // ---------------- UI MESSAGES ----------------
        public string UsernameMessage { get; private set; }
        public string PasswordMessage { get; private set; }
        public string ConfirmPasswordMessage { get; private set; }
        public Brush ConfirmPasswordColor { get; private set; } = Brushes.Transparent;

        public string Message { get; private set; }
        public Brush MessageColor { get; private set; } = Brushes.Transparent;

        // ---------------- COMMANDS ----------------
        public ICommand ResetPasswordCommand { get; }
        public ICommand ReturnToMainCommand { get; }

        // ---------------- LOGIC ----------------

        private void LoadSecurityQuestion()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                SecurityQuestion = "";
                UsernameMessage = "Username cannot be empty.";
            }
            else
            {
                var question = _userManager.GetSecurityQuestion(Username);
                if (question == null)
                {
                    SecurityQuestion = "";
                    UsernameMessage = "User not found.";
                }
                else
                {
                    SecurityQuestion = question;
                    UsernameMessage = "";
                }
            }

            OnPropertyChanged(nameof(SecurityQuestion));
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
                ConfirmPasswordColor = Brushes.Black;
            }
            else if (Password == ConfirmPassword)
            {
                ConfirmPasswordMessage = "Passwords match ✅";
                ConfirmPasswordColor = Brushes.DarkGreen;
            }
            else
            {
                ConfirmPasswordMessage = "Passwords do not match ❌";
                ConfirmPasswordColor = Brushes.Red;
            }

            OnPropertyChanged(nameof(ConfirmPasswordMessage));
            OnPropertyChanged(nameof(ConfirmPasswordColor));
        }

        public void Invalidate()
        {
            ValidatePassword();
            ValidatePasswordsMatch();
            CommandManager.InvalidateRequerySuggested();
        }

        private async void ResetPassword(object _)
        {
            var (isValid, _) = UserManager.IsPasswordValid(Password);
            if (!isValid || Password != ConfirmPassword)
                return;

            var (ok, msg) = _userManager.ResetPasswordWithSecurity(Username, SecurityAnswer, Password);

            Message = msg;
            MessageColor = ok ? Brushes.DarkGreen : Brushes.Red;
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(MessageColor));

            if (ok)
            {
                await Task.Delay(800);

                var main = new MainWindow { DataContext = new MainViewModel(_userManager) };
                main.Show();

                foreach (var w in Application.Current.Windows)
                    if (w is ForgotPasswordWindow fp)
                        fp.Close();
            }
        }

        private bool CanResetPassword(object _)
        {
            var (isValid, _) = UserManager.IsPasswordValid(Password);

            return !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(SecurityQuestion)
                && !string.IsNullOrWhiteSpace(SecurityAnswer)
                && isValid
                && Password == ConfirmPassword;
        }

        private void ReturnToMain(object _)
        {
            var main = new MainWindow { DataContext = new MainViewModel(_userManager) };
            main.Show();

            foreach (var w in Application.Current.Windows)
                if (w is ForgotPasswordWindow fp)
                    fp.Close();
        }

        // ---------------- PROPERTYCHANGED ----------------
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}