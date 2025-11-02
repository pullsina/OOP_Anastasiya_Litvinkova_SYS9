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
        public string Message { get; set; }
        public ICommand LoginCommand { get; }
        public ICommand OpenRegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        private readonly UserManager _userManager;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //Constructor
        public MainViewModel()
        {
            _userManager = App.SharedUserManager;
            LoginCommand = new RelayCommand(Login, CanLogin);
            OpenRegisterCommand = new RelayCommand(OpenRegister);
            ForgotPasswordCommand = new RelayCommand(ResetPassword);
        }
        public MainViewModel(UserManager sharedManager)
        {
            _userManager = sharedManager;
            LoginCommand = new RelayCommand(Login, CanLogin);
            OpenRegisterCommand = new RelayCommand(OpenRegister);
            ForgotPasswordCommand = new RelayCommand(ResetPassword);
        }


        //Methods
        private async void Login(object parameter)
        {
            string password = Password ?? "";
            bool success = _userManager.Login(Username, password);
            if (success)
            {
                //Open RecipeListWindow
                Message = "Login successful!";
                OnPropertyChanged(nameof(Message));
                await Task.Delay(500); // halv sekunds paus
                var recipeList = new RecipeListWindow();
                recipeList.Show();

                //Closing MainWindow
                foreach (var w in System.Windows.Application.Current.Windows)
                {
                    if (w is MainWindow main) main.Close();
                }
            }
            else
            {
                Message = "Invalid username or password.";
                OnPropertyChanged(nameof(Message));
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
    }

}
