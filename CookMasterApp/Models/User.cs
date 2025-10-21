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

        //Constructor
        public User (string Username, string Password, string Country)
        {
            this.Username = Username;
            this.Password = Password;
            this.Country = Country;
        }

        //Methods 
        //ValidateLogin()
        //ChangePassword()
        //UpdateDitalis()
    }
}
