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
    internal class UserManager : INotifyPropertyChanged
    {
        private User _loggedInUser;
        private readonly List<User> _users = new();
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

        //Methods
        public bool Login(string username, string password)
        {
            foreach (User user in _users)
            {
                if (string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase) && user.Password == password)
                {
                    LoggedInUser = user;
                    return true;
                }
            }
            return false;
        }
        public void Logout()
        {
            LoggedInUser = null;
        }

        public bool Register(string username, string password, string country)
        {
            //foreach (User user in _users)
            //{
            //    if (string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase))
            //        return false;
            //}

            if (_users.Any(usr => string.Equals(usr.Username, username, StringComparison.OrdinalIgnoreCase))
                || string.IsNullOrWhiteSpace(username))
                return false;
            else if (password.Length < 8 || !password.Any(Char.IsDigit) || !password.Any(Char.IsSymbol)
                || string.IsNullOrWhiteSpace(password))
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







        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
