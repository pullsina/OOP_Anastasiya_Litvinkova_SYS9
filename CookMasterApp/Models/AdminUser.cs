using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMasterApp.Models
{
    internal class AdminUser : User
    {
        //Constructor
        public AdminUser(string Username, string Password, string Country) : base(Username, Password, Country) { }
        //Methods
        //RemoveAnyRecipe()
        //viewAllRecipes()
    }
}
