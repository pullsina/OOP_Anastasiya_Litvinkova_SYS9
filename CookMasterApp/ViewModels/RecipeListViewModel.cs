using CookMasterApp.Managers;
using CookMasterApp.Models;
using CookMasterApp.Views;
using MVVM_KlonaMIg.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CookMasterApp.ViewModels
{
    internal class RecipeListViewModel : INotifyPropertyChanged
    {
        //===========PROPS==================

        private readonly RecipeManager _recipeManager;
        private readonly UserManager _userManager;
        public string UserName { get;}
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

        //==========CONSTRUCTOR============
        public RecipeListViewModel()
        {
            _recipeManager = App.SharedRecipeManager;//using the same manager
            _userManager = App.SharedUserManager;

            var currentUser = _userManager.GetLoggedInUser();
            if (currentUser is AdminUser)
                Recipes = new ObservableCollection<Recipe>(_recipeManager.GetAllRecipes());
            else
                Recipes = new ObservableCollection<Recipe>(_recipeManager.GetRecipesByUser(currentUser));

            UserName = currentUser.Username;
            OnPropertyChanged(nameof(UserName));

            AddRecipeCommand = new RelayCommand(OpenAddRecipe);
            ViewRecipeCommand = new RelayCommand(OpenRecipeDetails);
            RemoveRecipeCommand = new RelayCommand(RemoveSelectedRecipe);
            UserCommand = new RelayCommand(OpenUserDetails);
            SignOutCommand = new RelayCommand(SignOut);
            InfoCommand = new RelayCommand(ShowInfo);
            SearchCommand = new RelayCommand(SearchRecipes);
            ClearSearchCommand = new RelayCommand(ClearSearch);
            _recipeManager.PropertyChanged += (sender, args) =>
            {
                RefreshRecipes();
            }; //Updating the recipe list in UI when RecipeManager indicates that something has changed
        }
        //============METHODS==============
        private void SetMessage(string text, Brush color)
        {
            Message = text;
            MessageColor = color;
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(MessageColor));
        }

        private void OpenAddRecipe(object p)
        {
            var addWindow = new AddRecipeWindow();
            addWindow.Show();
        }

        private void OpenRecipeDetails(object p)
        {
            if (SelectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe first.", "No Recipe Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var detailWindow = new RecipeDetailWindow { DataContext = new RecipeDetailViewModel(SelectedRecipe) };
            detailWindow.Show();
        }

        private void RemoveSelectedRecipe(object p)
        {
            if (SelectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe to remove.", "No Recipe Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _recipeManager.RemoveRecipe(SelectedRecipe);
            Recipes.Remove(SelectedRecipe);
            SetMessage("Recipe removed.", Brushes.Red);
        }


        private void SearchRecipes(object p)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return;

            var currentUser = _userManager.GetLoggedInUser();
            var filtered = _recipeManager.Filter(SearchText);

            if (currentUser is not AdminUser)
                filtered = filtered.Where(r => r.CreatedBy == currentUser).ToList();

            Recipes = new ObservableCollection<Recipe>(filtered);
            OnPropertyChanged(nameof(Recipes));
        }

        private void RefreshRecipes()
        {
            var currentUser = _userManager.GetLoggedInUser();
            var list = currentUser is AdminUser
                ? _recipeManager.GetAllRecipes()
                : _recipeManager.GetRecipesByUser(currentUser);

            Recipes = new ObservableCollection<Recipe>(list);
            OnPropertyChanged(nameof(Recipes));
            Message = "Recipe list updated.";
            MessageColor = Brushes.DarkGreen;
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(MessageColor));

        }


        private void ClearSearch(object p)
        {
            SearchText = "";
            Recipes = new ObservableCollection<Recipe>(_recipeManager.GetAllRecipes());
            OnPropertyChanged(nameof(Recipes));
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
