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
    public class RecipeManager : INotifyPropertyChanged
    {
        private readonly List<Recipe> _recipes = new();
        public IEnumerable<Recipe> Recipes { get { return _recipes; } }


        //Constructor
        //Methods
        public void AddRecipe(Recipe recipe)
        {
            if (!_recipes.Contains(recipe))
            {
                _recipes.Add(recipe);
                OnPropertyChanged(nameof(Recipes));
            }
        }

        public void RemoveRecipe(Recipe recipe)
        {
            _recipes.Remove(recipe);
            OnPropertyChanged(nameof(Recipes));
        }

        public List<Recipe> GetAllRecipes()
        {
            return new List<Recipe>(_recipes); //skapar en klon av listan
        }

        public List<Recipe> GetRecipesByUser(User user)
        //{
        //   List <Recipe> userRecipes = new List<Recipe>();
        //   foreach (Recipe r in _recipes)
        //    {
        //       if (r.CreatedBy == user)
        //        {
        //            userRecipes.Add(r);
        //        }
        //    }
        //    return usersRecipes;      
        //}
        {
            return _recipes.Where(r => r.CreatedBy == user).ToList();
        }

        public List<Recipe> Filter(string criteria)
        {
            return _recipes.Where(r => r.Title.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                   r.Category.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                   r.Ingredients.Any(i => i.Contains(criteria, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        public void UpdateRecipe(Recipe recipeToUpdate)
        {
            recipeToUpdate.EditRecipe();
            OnPropertyChanged(nameof(Recipes));
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
