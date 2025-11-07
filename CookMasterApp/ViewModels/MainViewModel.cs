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

namespace CookMasterApp.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        //props
        private string _username;
       
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string Message { get; set; }
        public string MessageColor { get; set; }
        public ICommand LoginCommand { get; }
        public ICommand OpenRegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand VerifyCodeCommand { get; }

        private readonly UserManager _userManager;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private string _verificationCode;
        private string _enteredCode;
        private Visibility _codeFieldVisibility = Visibility.Collapsed;

        public string EnteredCode
        {
            get => _enteredCode;
            set
            {
                _enteredCode = value;
                OnPropertyChanged();
            }
        }

        public Visibility CodeFieldVisibility
        {
            get => _codeFieldVisibility;
            set
            {
                _codeFieldVisibility = value;
                OnPropertyChanged();
            }
        }

        //Constructor
        public MainViewModel()
        {
            _userManager = App.SharedUserManager;
            LoginCommand = new RelayCommand(Login, CanLogin);
            OpenRegisterCommand = new RelayCommand(OpenRegister);
            ForgotPasswordCommand = new RelayCommand(ResetPassword);
            VerifyCodeCommand = new RelayCommand(VerifyCode);

        }
        public MainViewModel(UserManager sharedManager)
        {
            _userManager = sharedManager;
            LoginCommand = new RelayCommand(Login, CanLogin);
            OpenRegisterCommand = new RelayCommand(OpenRegister);
            ForgotPasswordCommand = new RelayCommand(ResetPassword);
            VerifyCodeCommand = new RelayCommand(VerifyCode);

        }



        //Methods
        private void Login(object parameter)
        {
            string password = Password ?? "";

            // сбрасываем старые сообщения
            ErrorMessage = "";
            SuccessMessage = "";
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(SuccessMessage));

            // проверяем логин и пароль
            bool success = _userManager.Login(Username, password);

            if (success)
            {
                // не открываем окно рецептов сразу!
                // вместо этого запускаем имитацию отправки кода
                SendVerificationCode();
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        private bool CanLogin (object property)
        {            
            return !string.IsNullOrWhiteSpace(Username);
        }
        private void OpenRegister(object parameter)
        {
            //Open RegisterWindow
            var registerWindow = new RegisterWindow
            {
                DataContext = new RegisterViewModel(_userManager)
            };
            registerWindow.Show();
            //Close MainWindow
            foreach (var w in System.Windows.Application.Current.Windows)
            {
                if (w is MainWindow main) main.Close();
            }
        }
        private void ResetPassword(object parameter)
        {
            var forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.Show();
            foreach (var w in System.Windows.Application.Current.Windows)
            {
                if (w is MainWindow main) main.Close();
            }
        }
        private void SendVerificationCode()
        {
            var random = new Random();
            _verificationCode = random.Next(100000, 999999).ToString(); // шесть цифр
            CodeFieldVisibility = Visibility.Visible;

            // kopiera koden direkt till urklipp, annars blir det jobbigt att testa
            Clipboard.SetText(_verificationCode);

            // visar meddelande
            MessageBox.Show(
                $"Verification code (copied to clipboard): {_verificationCode}",
                "2FA Verification",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            //visa statusen i gränssnittet
            Message = "Verification code sent to your email (copied to clipboard).";
            MessageColor = "DarkSlateBlue";
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(MessageColor));
        }

        private void VerifyCode(object _)
        {
            if (_enteredCode == _verificationCode)
            {
                // lyckad kodkontroll öppnar RecipeListWindow
                var recipeList = new RecipeListWindow
                {
                    DataContext = new RecipeListViewModel()
                };
                recipeList.Show();

                foreach (var w in Application.Current.Windows)
                    if (w is MainWindow mw)
                        mw.Close();
            }
            else
            {
                Message = "Incorrect verification code.";
                MessageColor = "Red";
                OnPropertyChanged(nameof(Message));
                OnPropertyChanged(nameof(MessageColor));
            }
        }

    }

}
