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
        private  List<User> _users;


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
        public UserManager()
        {
            _users = new List<User>();
            User admin = new AdminUser("admin", "password", "Sweden", "What is your favorite food?", "Food");
            User defaultUser = new User("user", "password", "Sweden", "What is your favorite food?", "Milk");
            _users.Add(admin);
            _users.Add(defaultUser);
        }

        //Methods
        //public User? FindUser(string username)
        //{
        //    //foreach (User usr in _users)
        //    //{
        //    //    if (string.Equals(usr.Username, username, StringComparison.OrdinalIgnoreCase))
        //    //        return usr;
        //    //    return null;
        //    //}
        //    return _users.FirstOrDefault(usr => string.Equals(usr.Username, username, StringComparison.OrdinalIgnoreCase));
        //}

        public User? FindUser(string username)
        {
            System.Diagnostics.Debug.WriteLine($"DEBUG: Searching for user '{username}' in list of {_users.Count} users...");
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
        public static (bool isValid, string message) IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password cannot be empty.");
            if (password.Length < 8)
                return (false, "Password must be at least 8 characters.");
            if (!password.Any(char.IsDigit))
                return (false, "Password must contain at least one digit.");
            if (!password.Any(char.IsSymbol) && !password.Any(char.IsPunctuation))
                return (false, "Password must contain at least one symbol.");
            if (!password.Any(char.IsLetter))
                return (false, "Password must contain at least one letter.");
            return (true, "");
        }


        public (bool success, string message) Register(string username, string password, string country, string question, string answer)
        {
            User? user = FindUser(username);

            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(country)
                || string.IsNullOrWhiteSpace(question)
                || string.IsNullOrWhiteSpace(answer))
                return (false, "All fields are required.");

            if (user != null)
                return (false, "This username is already taken.");

            var (isValid, passwordMessage) = IsPasswordValid(password);
            if (!isValid)
                return (false, passwordMessage);

            User newUser = new User(username, password, country, question, answer);
            _users.Add(newUser);
            return (true, "Registration successful!");
        }


        public bool ChangePassword(string oldPassword, string newPassword)
        {
            if (LoggedInUser == null)
                return false;
            return LoggedInUser.ChangePassword(oldPassword, newPassword);
        }

        public User? GetLoggedInUser()
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

        public string? GetSecurityQuestion(string username)
    => FindUser(username)?.SecurityQuestion;

        public bool CheckSecurityAnswer(string username, string answer)
        {
            var u = FindUser(username);
            return u != null && string.Equals(u.SecurityAnswer, answer, StringComparison.OrdinalIgnoreCase);
        }

        public (bool ok, string message) ResetPasswordWithSecurity(string username, string answer, string newPassword)
        {
            var u = FindUser(username);
            if (u == null)
                return (false, "User not found.");
            if (!CheckSecurityAnswer(username, answer))
                return (false, "Wrong answer.");

            var (isValid, validationMsg) = IsPasswordValid(newPassword);
            if (!isValid)
                return (false, validationMsg);

            u.Password = newPassword;
            return (true, "Password has been reset.");
        }




        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
