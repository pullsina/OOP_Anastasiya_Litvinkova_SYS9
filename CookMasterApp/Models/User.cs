using CookMasterApp.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMasterApp.Models
{
    public class User
    {
        //Property
        public string Username { get; set; }
        public string Password { get; set; }
        public string Country { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }

        //Constructor
        public User (string Username, string Password, string Country, string SecurityQuestion, string securityAnswer)
        {
            this.Username = Username;
            this.Password = Password;
            this.Country = Country;
            this.SecurityQuestion = SecurityQuestion;
            this.SecurityAnswer = SecurityAnswer;
        }

        //Methods 
        //ValidateLogin()
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
            if (!UserManager.IsPasswordValid(newPassword))
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
    }
}
