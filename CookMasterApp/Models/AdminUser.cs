using CookMasterApp.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMasterApp.Models
{
    public class AdminUser : User
    {
        //Constructor
        public AdminUser(string username, string password, string country, string securityQuestion, string securityAnswer) : 
        base(username, password, country, securityQuestion, securityAnswer) { }
        //Methods

        //ChangePassword - finns inte i diagram
        public bool ChangePasswordByAdmin(UserManager manager, string username, string newPassword)
        {
            User? user = manager.FindUser(username);
            if (user == null)
                return false;

            var (isValid, message) = UserManager.IsPasswordValid(newPassword);
            if (!isValid)
            {
                return false;
            }

            user.Password = newPassword;
            return true;
        }

        //RemoveAnyRecipe()
        public void RemoveAnyRecipe (RecipeManager recipeManager, Recipe recipe)
        {
            recipeManager.RemoveRecipe(recipe);
        }
        //viewAllRecipes()
        public ObservableCollection<Recipe> ViewAllRecipes(RecipeManager recipeManager)
        {
            return recipeManager.GetAllRecipes();
        }

    }
}
