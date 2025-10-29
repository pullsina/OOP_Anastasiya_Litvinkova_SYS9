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
using System.Windows.Input;

namespace CookMasterApp.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        //props
        public string Username { get; set; }
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
            _userManager = new UserManager();
            LoginCommand = new RelayCommand(Login);
            OpenRegisterCommand = new RelayCommand(OpenRegister);
            ForgotPasswordCommand = new RelayCommand(ResetPassword);
        }

        //Methods
        private void Login(object parameter)
        {
            string password = parameter as string ?? ""; //telling WPF that our parameter is a string/password or ""
            bool success = _userManager.Login(Username, password);
            if (success)
            {
                //Open RecipeListWindow
                Message = "";
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
