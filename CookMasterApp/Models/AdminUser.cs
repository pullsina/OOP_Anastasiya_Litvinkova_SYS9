using CookMasterApp.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMasterApp.Models
{
    public class AdminUser : User
    {
        //Constructor
        public AdminUser(string Username, string Password, string Country) : base(Username, Password, Country) { }
        //Methods

        //ChangePassword - finns inte i diagram
        public bool ChangePasswordByAdmin (UserManager manager, string username, string newPassword)
        {
            User? user = manager.FindUser(username);
            if (user == null) 
                return false;
            if (!UserManager.IsPasswordValid(newPassword))
                return false;
            user.Password = newPassword;
            return true;
        }
        //RemoveAnyRecipe()

        //viewAllRecipes()
    }
}
