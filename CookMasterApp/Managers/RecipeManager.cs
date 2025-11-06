using CookMasterApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CookMasterApp.Managers
{
    public class RecipeManager : INotifyPropertyChanged
    {
        private readonly ObservableCollection<Recipe> _recipes = new();
        public ObservableCollection<Recipe> Recipes { get { return _recipes; } }


        //Constructor
        public RecipeManager()
        {
            // Tat existerande defaultUser
            var defaultUser = App.SharedUserManager?
                .FindUser("user"); 

            if (defaultUser == null)
                return; // just in case:)

            var pancakes = new Recipe(
                "Pancakes",
                new List<string> { "2 eggs", "2 dl milk", "1 dl flour", "Butter", "Sugar" },
                "Whisk eggs and milk, add flour, fry thin pancakes in butter.",
                "Dessert",
                defaultUser);

            var pasta = new Recipe(
                "Garlic Pasta",
                new List<string> { "200 g spaghetti", "2 cloves garlic", "Olive oil", "Salt" },
                "Boil pasta, sauté garlic in oil, mix together, season with salt.",
                "Main dish",
                defaultUser);

            _recipes.Add(pancakes);
            _recipes.Add(pasta);
        }
        //Methods
        public void AddRecipe(Recipe recipe)
        {
            if (!_recipes.Contains(recipe))
            {
                _recipes.Add(recipe);
            }
        }

        public void RemoveRecipe(Recipe recipe)
        {
            _recipes.Remove(recipe);
        }

        public ObservableCollection<Recipe> GetAllRecipes()
        {
            return new ObservableCollection<Recipe>(_recipes); //skapar en klon av listan
        }

        public ObservableCollection<Recipe> GetRecipesByUser(User user)
        {
            ObservableCollection<Recipe> userRecipes = new ObservableCollection<Recipe>();
            foreach (Recipe r in _recipes)
            {
                if (r.CreatedBy == user)
                {
                    userRecipes.Add(r);
                }
            }
            return userRecipes;
        }


        public ObservableCollection<Recipe> Filter(string criteria)
        {
            if (string.IsNullOrWhiteSpace(criteria))
                return new ObservableCollection<Recipe>(_recipes);

            criteria = criteria.Trim();

            // tolkar söktexten som datum
            if (DateTime.TryParse(criteria, out var searchDate))
            {
                return new ObservableCollection<Recipe>(_recipes.Where(r => r.Date.Date ==searchDate.Date));
            }

            // tolka söktexten som ett tal (t.ex. dag eller månad)
            if (int.TryParse(criteria, out int number))
            {
                return new ObservableCollection<Recipe> (_recipes.Where(r =>
                    r.Date.Day == number ||
                    r.Date.Month == number ||
                    r.Title.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                    r.Category.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                    r.Ingredients.Any(i => i.Contains(criteria, StringComparison.OrdinalIgnoreCase))
                ));
            }

            // Annars vanlig textfiltrering
            return new ObservableCollection<Recipe>(_recipes.Where(r =>
                r.Title.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                r.Category.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                r.Ingredients.Any(i => i.Contains(criteria, StringComparison.OrdinalIgnoreCase)) ||
                r.Date.ToString("yyyy-MM-dd").Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                r.Date.ToString("dd/MM/yyyy").Contains(criteria, StringComparison.OrdinalIgnoreCase)
            ));
        }


        public void UpdateRecipe(Recipe recipeToUpdate)
        {
            recipeToUpdate.EditRecipe();
            //OnPropertyChanged(nameof(Recipes));
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
