using CookMasterApp.Managers;
using CookMasterApp.Views;
using MVVM_KlonaMIg.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
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

        // PROPERTIES
        private readonly UserManager _userManager;
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                ValidateUsername();//för att få meddelande direkt vid unmätning 
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();//Det gör att WPF kollar om
                //knappar som är bundna till ICommand ska vara aktiva eller inaktiva
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

      
        // VALIDATION & MESSAGES
      

        public string Message { get; set; }
        public string MessageColor { get; set; } = "Red"; // Default red (error)
        public string UsernameMessage { get; set; }
        public string PasswordMessage { get; set; }
        public string ConfirmPasswordMessage { get; set; }
        private string _confirmPasswordColor = "Black";
        public string ConfirmPasswordColor//ändrar på text färg från "inuti"
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
                ValidatePassword();//kontrollerar passwords direkt vid inmätning 
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
        public ICommand RegisterCommand { get; }
        public ICommand ReturnToMainCommand { get; }

       
        // CONSTRUCTORS
       

        public RegisterViewModel()
        {
            _userManager = App.SharedUserManager;
            RegisterCommand = new RelayCommand(Register, CanRegister);
            ReturnToMainCommand = new RelayCommand(ReturnToMain);
        }

 
        // VALIDATION METHODS  

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
            // Anropar metoden IsPasswordValid i UserManager, som returnerar en "tuple":
            // ett bool-värde (isValid) och ett meddelande (message).
            PasswordMessage = isValid ? "" : message;
            // Om isValid är true => inget felmeddelande,
            // annars visas meddelandet som kom från UserManager.
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


  
        // COMMAND METHODS
       

        private async void Register(object parameter)//async så att utförandet av ett kommando kan fördröjas
        {
            string password = parameter is PasswordBox pb ? pb.Password : "";
            //Om parameter är ett PasswordBox-objekt
            //ta texten från dess Password fält,
            //annars använd en tom sträng.

            var (success, message) = _userManager.Register(Username, password, SelectedCountry, SelectedSecurityQuestion, SecurityAnswer);

            Message = message;
            MessageColor = success ? "Green" : "Red";
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(MessageColor));

            if (success)
            {
                await Task.Delay(700); // låt användaren se "Registration successful!"
                var mainWindow = new MainWindow();
                mainWindow.Show();

                foreach (var w in Application.Current.Windows)
                {
                    if (w is RegisterWindow reg) reg.Close();
                }
            }
        }

        private bool CanRegister(object parameter)
        {
            // kntrollerar att parameter verkligen är en PasswordBox.
            // Om iinte avsluta metoden (return false) eftersom vi inte kan läsa något lösenord.
            if (parameter is not PasswordBox passwordBox)
                return false;
            // Hämtar det första lösenordet som användaren skrev in
            var mainPassword = passwordBox.Password;
            // Hämtar det aktuella RegisterWindow från alla öppna fönster i appen.
            var registerWindow = Application.Current.Windows.OfType<RegisterWindow>().FirstOrDefault();
            //Letar upp den andra PasswordBox(PasswordBox2) i fönstret,
            // dvs. rutan där användaren bekräftar lösenordet.
            var confirmPasswordBox = registerWindow?.FindName("PasswordBox2") as PasswordBox;
            // Hämtar texten från den andra lösenordsrutan.
            // Om den är null (t.ex. rutan hittades inte), används en tom sträng istället.
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
            var mainWindow = new MainWindow();
            mainWindow.Show();

            foreach (var w in Application.Current.Windows)
            {
                if (w is RegisterWindow reg) reg.Close();
            }
        }

      
        // INotifyPropertyChanged
      

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
