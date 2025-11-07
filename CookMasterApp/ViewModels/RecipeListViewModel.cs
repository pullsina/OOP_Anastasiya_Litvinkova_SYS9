using CookMasterApp.Managers;
using CookMasterApp.Models;
using CookMasterApp.Views;
using MVVM_KlonaMIg.MVVM;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CookMasterApp.ViewModels
{
    internal class RecipeListViewModel : INotifyPropertyChanged
    {
        private readonly RecipeManager _recipeManager;
        private readonly UserManager _userManager;

        public User CurrentUser => _userManager.GetLoggedInUser();

        public ObservableCollection<Recipe> Recipes { get; set; }
        public Recipe SelectedRecipe { get; set; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public string Message { get; set; }
        public Brush MessageColor { get; set; }

        public ICommand AddRecipeCommand { get; }
        public ICommand ViewRecipeCommand { get; }
        public ICommand RemoveRecipeCommand { get; }
        public ICommand UserCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand InfoCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }

        // ---------- CONSTRUCTOR ----------
        public RecipeListViewModel()
        {
            _recipeManager = App.SharedRecipeManager;
            _userManager = App.SharedUserManager;

            var currentUser = _userManager.GetLoggedInUser();

            if (currentUser is AdminUser)
            {
                // skapar en oberoende samling baserad på alla recept (annars krashar)
                Recipes = new ObservableCollection<Recipe>(_recipeManager.Recipes);
            }
            else
            {
                // vanlig user ser båra sina resept
                Recipes = new ObservableCollection<Recipe>(
                    _recipeManager.Recipes.Where(r => r.CreatedBy == currentUser));
            }

            // uppdaterar lista efter add/remove
            _recipeManager.Recipes.CollectionChanged += (s, e) =>
            {
                RefreshFilteredRecipes();
            };

            AddRecipeCommand = new RelayCommand(OpenAddRecipe);
            ViewRecipeCommand = new RelayCommand(OpenRecipeDetails);
            RemoveRecipeCommand = new RelayCommand(RemoveSelectedRecipe);
            UserCommand = new RelayCommand(OpenUserDetails);
            SignOutCommand = new RelayCommand(SignOut);
            InfoCommand = new RelayCommand(ShowInfo);
            SearchCommand = new RelayCommand(SearchRecipes);
            ClearSearchCommand = new RelayCommand(ClearSearch);
        }

        // ---------- METHODS ----------

        private void OpenAddRecipe(object p)
        {
            var addWindow = new AddRecipeWindow();
            addWindow.Show();
        }

        private void OpenRecipeDetails(object p)
        {
            if (SelectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe first.", "No Recipe Selected",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var detailWindow = new RecipeDetailWindow
            {
                DataContext = new RecipeDetailViewModel(SelectedRecipe)
            };
            detailWindow.Show();
        }

        private void RemoveSelectedRecipe(object p)
        {
            if (SelectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe to remove.", "No Recipe Selected",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _recipeManager.RemoveRecipe(SelectedRecipe);
            Recipes.Remove(SelectedRecipe);
            SetMessage("Recipe removed.", Brushes.Red);
        }

        private void SearchRecipes(object p)
        {
            var currentUser = _userManager.GetLoggedInUser();

            // Vi får redan en oberoende lista
            var filtered = new ObservableCollection<Recipe>(
                _recipeManager.Filter(SearchText ?? "")
                .Where(r => currentUser is AdminUser || r.CreatedBy == currentUser)
                .OrderByDescending(r => r.Date));

            Recipes.Clear();
            foreach (var r in filtered)
                Recipes.Add(r);

            SetMessage(
                Recipes.Count > 0
                    ? $"Found {Recipes.Count} recipe{(Recipes.Count == 1 ? "" : "s")}."
                    : "No recipes found.",
                Brushes.DarkSlateBlue
            );
        }






        private void ClearSearch(object p)
        {
            SearchText = "";
            RefreshFilteredRecipes();
            SetMessage("Search cleared.", Brushes.Gray);
        }

        private void RefreshFilteredRecipes()
        {
            var currentUser = _userManager.GetLoggedInUser();

            var filtered = currentUser is AdminUser
                ? _recipeManager.Recipes
                : new ObservableCollection<Recipe>(_recipeManager.Recipes.Where(r => r.CreatedBy == currentUser));

            Recipes.Clear();
            foreach (var r in filtered)
                Recipes.Add(r);
        }

        private void ShowInfo(object p)
        {
            MessageBox.Show("CookMaster - Your personal recipe manager.\nDeveloped as part of the Newton YH SYSM9 project.");
        }

        private void OpenUserDetails(object p)
        {
            var userWindow = new UserDetailWindow();
            userWindow.Show();
        }

        private void SignOut(object p)
        {
            _userManager.Logout();
            var main = new MainWindow { DataContext = new MainViewModel(_userManager) };
            main.Show();
            foreach (var w in Application.Current.Windows)
                if (w is RecipeListWindow rl) rl.Close();
        }

        private void SetMessage(string text, Brush color)
        {
            Message = text;
            MessageColor = color;
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(MessageColor));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
