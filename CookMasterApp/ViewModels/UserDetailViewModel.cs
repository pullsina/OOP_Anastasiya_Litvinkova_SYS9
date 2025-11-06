using CookMasterApp.Managers;
using CookMasterApp.Models;
using CookMasterApp.Views;
using MVVM_KlonaMIg.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CookMasterApp.ViewModels
{
    internal class UserDetailViewModel : INotifyPropertyChanged
    {
        private readonly UserManager _userManager;
        private readonly User _currentUser;

        // --- INPUTS ---
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; 
                ValidateUsername();
                OnPropertyChanged(); }
        }

        private string _selectedCountry;
        public string SelectedCountry
        {
            get => _selectedCountry;
            set { _selectedCountry = value; OnPropertyChanged(); }
        }

        public List<string> Countries { get; } = new()
        {
            "Sweden", "Norway", "Denmark"
        };

        public string OldPassword { get; set; }
        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                ValidatePassword();
                ValidatePasswordsMatch();
                OnPropertyChanged();
            }
        }
        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                ValidatePassword();
                ValidatePasswordsMatch();
                OnPropertyChanged();
            }
        }


        // --- UI MESSAGES ---
        public string UsernameMessage { get; private set; }
        public string CountryMessage { get; private set; }
        public string OldPasswordMessage { get; private set; }
        public string PasswordMessage { get; private set; }
        public string ConfirmPasswordMessage { get; private set; }
        public Brush ConfirmPasswordColor { get; private set; } = Brushes.Transparent;

        public string Message { get; private set; }
        public Brush MessageColor { get; private set; } = Brushes.Transparent;

        // --- COMMANDS ---
        public ICommand SaveDetailsCommand { get; }
        public ICommand BackCommand { get; }

        // --- CONSTRUCTOR ---
        public UserDetailViewModel()
        {
            _userManager = App.SharedUserManager;
            _currentUser = _userManager.GetLoggedInUser();

            Username = _currentUser.Username;
            SelectedCountry = _currentUser.Country;

            SaveDetailsCommand = new RelayCommand(SaveDetails);
            BackCommand = new RelayCommand(BackToList);
        }

        // --- METHODS ---
        private void SaveDetails(object _)
        {
            ClearMessages();

            // Kontrollera tomma fält
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(SelectedCountry))
            {
                Message = "Please fill in all required fields.";
                MessageColor = Brushes.Red;
                OnPropertyChanged(nameof(Message));
                OnPropertyChanged(nameof(MessageColor));
                return;
            }

            // Kollar om användarnamnet är upptaget
            var existingUser = _userManager.FindUser(Username);
            if (existingUser != null && existingUser != _currentUser)
            {
                UsernameMessage = "This username is already taken.";
                OnPropertyChanged(nameof(UsernameMessage));
                return;
            }

            // Kolla giltighet på nytt lösenord (om angivet)
            if (!string.IsNullOrWhiteSpace(NewPassword) || !string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                var (isValid, msg) = UserManager.IsPasswordValid(NewPassword);
                if (!isValid)
                {
                    PasswordMessage = msg;
                    OnPropertyChanged(nameof(PasswordMessage));
                    return;
                }

                if (NewPassword != ConfirmPassword)
                {
                    ConfirmPasswordMessage = "Passwords do not match.";
                    ConfirmPasswordColor = Brushes.Red;
                    OnPropertyChanged(nameof(ConfirmPasswordMessage));
                    OnPropertyChanged(nameof(ConfirmPasswordColor));
                    return;
                }
            }

            // Verifiera det gamla lösenordet
            if (string.IsNullOrWhiteSpace(OldPassword) || !_currentUser.ValidateLogin(_currentUser.Username, OldPassword))
            {
                OldPasswordMessage = "Incorrect current password.";
                OnPropertyChanged(nameof(OldPasswordMessage));
                return;
            }

            // Utför uppdateringen
            bool success = _userManager.UpdateDetails(_currentUser.Username, Username, SelectedCountry);
            if (!success)
            {
                Message = "Username already exists.";
                MessageColor = Brushes.Red;
                OnPropertyChanged(nameof(Message));
                OnPropertyChanged(nameof(MessageColor));
                return;
            }

            // Ändra lösenord om nytt angavs
            if (!string.IsNullOrWhiteSpace(NewPassword))
                _userManager.ChangePassword(OldPassword, NewPassword);

            Message = "Details updated successfully!";
            MessageColor = Brushes.DarkGreen;
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(MessageColor));

            MessageBox.Show("Your details have been updated!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);

            // Stäng fönstret
            foreach (var w in Application.Current.Windows)
                if (w is UserDetailWindow uw) uw.Close();
        }

        private void ClearMessages()
        {
            UsernameMessage = "";
            CountryMessage = "";
            OldPasswordMessage = "";
            PasswordMessage = "";
            ConfirmPasswordMessage = "";
            ConfirmPasswordColor = Brushes.Transparent;
            OnPropertyChanged(nameof(UsernameMessage));
            OnPropertyChanged(nameof(CountryMessage));
            OnPropertyChanged(nameof(OldPasswordMessage));
            OnPropertyChanged(nameof(PasswordMessage));
            OnPropertyChanged(nameof(ConfirmPasswordMessage));
            OnPropertyChanged(nameof(ConfirmPasswordColor));
        }

        private void BackToList(object _)
        {
            foreach (var w in Application.Current.Windows)
                if (w is UserDetailWindow uw) uw.Close();
        }

        private void ValidateUsername()
        {
            if (string.IsNullOrWhiteSpace(Username))
                UsernameMessage = "Username cannot be empty.";
            else if (Username.Length < 3)
                UsernameMessage = "Username must be at least 3 characters.";
            else
                UsernameMessage = "";
            OnPropertyChanged(nameof(UsernameMessage));
        }

        private void ValidatePassword()
        {
            var (isValid, message) = UserManager.IsPasswordValid(NewPassword);
            PasswordMessage = isValid ? "" : message;
            OnPropertyChanged(nameof(PasswordMessage));
        }

        private void ValidatePasswordsMatch()
        {
            if (string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
            {
                ConfirmPasswordMessage = "";
                ConfirmPasswordColor = Brushes.Black;
            }
            else if (NewPassword == ConfirmPassword)
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
