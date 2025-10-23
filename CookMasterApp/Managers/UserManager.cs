using CookMasterApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CookMasterApp.Managers
{
    public class UserManager : INotifyPropertyChanged
    {
        private User? _loggedInUser;
        private readonly List<User> _users = new() ;
        public bool IsAuthenticated => _loggedInUser != null;
        public User? LoggedInUser
        {
            get { return _loggedInUser; }
            private set
            {
                _loggedInUser = value;
                OnPropertyChanged(nameof(LoggedInUser));
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }
        //Constructor
        public UserManager ()
        {
            User admin = new AdminUser("admin", "password", "Sweden");
            User defaultUser = new User("user", "password", "Sweden");
            _users.Add(admin);
            _users.Add(defaultUser);
        }
        
        //Methods
        public User? FindUser(string username)
        {
            //foreach (User usr in _users)
            //{
            //    if (string.Equals(usr.Username, username, StringComparison.OrdinalIgnoreCase))
            //        return usr;
            //    return null;
            //}
            return _users.FirstOrDefault(usr => string.Equals(usr.Username, username, StringComparison.OrdinalIgnoreCase));
        }

        public bool Login(string username, string password)
        {
            User? user = FindUser(username);
            
                if (user != null && user.ValidateLogin(username, password))
                {
                    LoggedInUser = user;
                    return true;
                }
            
            return false;
        }
        public void Logout()
        {
            LoggedInUser = null;
        }

        //IsPasswordValid (finns inte i diagrammet)
        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            bool hasDigit = password.Any(char.IsDigit);
            bool hasSymbol = password.Any(char.IsSymbol) || password.Any(char.IsPunctuation);
            bool hasLetter = password.Any(char.IsLetter);
            return password.Length >= 8 && hasDigit && hasSymbol && hasLetter;
        }

        public bool Register(string username, string password, string country)
        {
            User? user = FindUser(username);
            if (string.IsNullOrWhiteSpace(username))
                return false;
            else if (user != null)
            {
                return false;
            }
            else if (!IsPasswordValid(password))
                return false;
            else if (string.IsNullOrWhiteSpace(country))
                return false;
            else
            {
                User newUser = new User(username, password, country);
                _users.Add(newUser);
                return true;
            }
        }

        public bool ChangePassword (string oldPassword, string newPassword)
        {
            if (LoggedInUser == null)
                return false;
            return LoggedInUser.ChangePassword(oldPassword, newPassword);
        }
      
        public User? GetLoggedInUser ()
        {
            return LoggedInUser;
        }

        public bool UpdateDetails(string currentUsername, string newUsername, string newCountry)
        {
            if (LoggedInUser == null)
                return false;

            bool occupied = _users.Any(u => !u.Username.Equals(LoggedInUser.Username, StringComparison.OrdinalIgnoreCase) && string.Equals(u.Username, newUsername, StringComparison.OrdinalIgnoreCase));
            if (occupied)
                return false;
            return LoggedInUser.UpdateDetails(newUsername, newCountry);
        }




        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
