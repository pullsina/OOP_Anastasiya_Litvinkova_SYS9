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
        private readonly UserManager _userManager;
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
        //här har jag lite blandade messages
        //Jag borde bara ha lämnat Message och MessageColor, men det finns ingen tid nu

        //Props för ICommands
        public ICommand LoginCommand { get; }
        public ICommand OpenRegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand VerifyCodeCommand { get; }
 
        //Props för 2FA
        public string EnteredCode
        {
            get => _enteredCode;
            set
            {
                _enteredCode = value;
                OnPropertyChanged();
            }
        }
        // Visibility används för att visa, gömma eller dölja WPFelement dynamiskt
        //t.ex.när användaren loggar in, klickar på en knapp eller ska bekräfta något(som 2FA kodruta dyker upp).
        public Visibility CodeFieldVisibility 
        {
            get => _codeFieldVisibility;
            set
            {
                _codeFieldVisibility = value;
                OnPropertyChanged();
            }
        }

        //--------CONSTRUCTOR-----------
        public MainViewModel()
        {
            _userManager = App.SharedUserManager;
            LoginCommand = new RelayCommand(Login, CanLogin);
            OpenRegisterCommand = new RelayCommand(OpenRegister);
            ForgotPasswordCommand = new RelayCommand(ResetPassword);
            VerifyCodeCommand = new RelayCommand(VerifyCode);
        }


        //Methods
        private void Login(object parameter)
        {
            string password = Password ?? ""; //Om Password är null => använd en tom sträng istället.
            //annars kan krasha

            // rensar gamla meddelande
            ErrorMessage = "";
            SuccessMessage = "";
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(SuccessMessage));

            // kontrollerar username och lösenord
            bool success = _userManager.Login(Username, password);

            if (success)
            {
                // Öppna inte RecipeList direkt
                // istället kör en simulering av kodsändning
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
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }
        private void OpenRegister(object parameter)
        {
            //Open RegisterWindow
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            //Close MainWindow
            foreach (var w in System.Windows.Application.Current.Windows)
            {
                if (w is MainWindow main) main.Close();
            }
            // Koden går igenom alla fönster som är öppna i programmet just nu.
            // Om något av dem är MainWindow, så stängs det.
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
            var random = new Random(); //slumpar ett tal
            _verificationCode = random.Next(100000, 999999).ToString();//ToString eftersom EnteredCode är string
            CodeFieldVisibility = Visibility.Visible; //gör fältet med kodinmätning sinligt

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

        private void VerifyCode(object p)
        {
            if (_enteredCode == _verificationCode)
            {
                // lyckad kodkontroll öppnar RecipeListWindow
                var recipeList = new RecipeListWindow();
                recipeList.Show();
                //Closing Main
                foreach (var w in Application.Current.Windows)
                    if (w is MainWindow main)main.Close();
            }
            else
            {
                Message = "Incorrect verification code.";
                MessageColor = "Red";
                OnPropertyChanged(nameof(Message));
                OnPropertyChanged(nameof(MessageColor));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private string _verificationCode;
        private string _enteredCode;
        private Visibility _codeFieldVisibility = Visibility.Collapsed;
    }

}
