using CookMasterApp.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CookMasterApp.Models
{
    public class User : INotifyPropertyChanged //för att Username uppdaterades i RecipeList
                                               //när man ändrar den i UserDetails
    {
        //Property
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }
        public string Password { get; set; }
        public string Country { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }

        //Constructor
        public User (string Username, string Password, string Country, string SecurityQuestion, string SecurityAnswer)
        {
            this.Username = Username;
            this.Password = Password;
            this.Country = Country;
            this.SecurityQuestion = SecurityQuestion;
            this.SecurityAnswer = SecurityAnswer;
        }

        //Methods 
        public bool ValidateLogin (string username, string password)
        {
            return string.Equals(Username, username, StringComparison.OrdinalIgnoreCase)
                && Password == password;
        }


        //ChangePassword()
        public bool ChangePassword(string oldPassword, string newPassword)
        {
            if (Password != oldPassword)
                return false;

            var (isValid, message) = UserManager.IsPasswordValid(newPassword);
            if (!isValid)
                return false;

            Password = newPassword;
            return true;
        }

        //UpdateDitalis()
        public bool UpdateDetails(string newUsername, string newCountry)
        {
           if (string.IsNullOrWhiteSpace(newUsername) || newUsername.Length < 3)
                return false;
            if (string.IsNullOrWhiteSpace(newCountry))
                return false;

            Username = newUsername.Trim();
            Country = newCountry;
            return true;                    
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
