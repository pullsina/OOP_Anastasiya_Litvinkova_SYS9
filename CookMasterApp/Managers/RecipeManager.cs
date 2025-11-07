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
                OnPropertyChanged(nameof(Recipes));
            }
        }

        public void RemoveRecipe(Recipe recipe)
        {
            if (_recipes.Contains(recipe))
            {
                _recipes.Remove(recipe);
                OnPropertyChanged(nameof(Recipes));
            }
        }

        public ObservableCollection<Recipe> GetAllRecipes()
        {
            return _recipes;
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

        // Filtrerar recepten baserat på användarens söktext (criteria).
        // Returnerar alltid en "ny" ObservableCollection så att UI uppdateras korrekt,
        // och för att undvika att manipulera den ursprungliga listan direkt.
        public ObservableCollection<Recipe> Filter(string criteria)
        {
            // Om sökfältet är tomt eller bara innehåller mellanslag returnerar alla recept.
            if (string.IsNullOrWhiteSpace(criteria))
                return new ObservableCollection<Recipe>(_recipes);

            // Tar bort onödiga mellanslag runt söktexten.
            criteria = criteria.Trim();

            // Försöker tolka söktexten som ett datum (t.ex. "2025-11-07").
            // Om det lyckas returnerar recept med samma datum.
            if (DateTime.TryParse(criteria, out var searchDate))
            {
                return new ObservableCollection<Recipe>(
                    _recipes.Where(r => r.Date.Date == searchDate.Date)
                            .OrderByDescending(r => r.Date));
            }

            // Om användaren skrev in ett fyrsiffrigt tal (t.ex. "2024"),
            // tolkar vi det som ett år och visar recept från det året
            if (criteria.Length == 4 && int.TryParse(criteria, out int year))
            {
                return new ObservableCollection<Recipe>(
                    _recipes.Where(r => r.Date.Year == year)
                            .OrderByDescending(r => r.Date));
            }

            // Annars söker vi efter textmatchningar i titel, kategori eller ingredienser,
            // samt i receptdatum i olika format.
            return new ObservableCollection<Recipe>(
                _recipes.Where(r =>
                    r.Title.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                    r.Category.Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                    r.Ingredients.Any(i => i.Contains(criteria, StringComparison.OrdinalIgnoreCase)) ||
                    r.Date.ToString("yyyy-MM-dd").Contains(criteria, StringComparison.OrdinalIgnoreCase) ||
                    r.Date.ToString("dd/MM/yyyy").Contains(criteria, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(r => r.Date) // sorterar recepten så att de nyaste visas först
            );
        }

        public void UpdateRecipe(Recipe recipeToUpdate)
        {
            recipeToUpdate.EditRecipe();
            OnPropertyChanged(nameof(Recipes));
        }

        // Uppdaterar ett befintligt recept.
        // Anropar metodens EditRecipe() på det valda receptet för att spara ändringar
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
