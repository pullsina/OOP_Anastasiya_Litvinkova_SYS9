using CookMasterApp.Managers;
using CookMasterApp.Models;
using CookMasterApp.Views;
using MVVM_KlonaMIg.MVVM;
using System;
using System.Collections.Generic;
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
    internal class AddRecipeViewModel : INotifyPropertyChanged
    {
        private readonly RecipeManager _recipeManager;
        private readonly UserManager _userManager;

        // ========= PROPERTIES =========

        private string _title;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _category;
        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        private string _ingredients;
        public string Ingredients
        {
            get => _ingredients;
            set { _ingredients = value; OnPropertyChanged(); }
        }

        private string _instructions;
        public string Instructions
        {
            get => _instructions;
            set { _instructions = value; OnPropertyChanged(); }
        }

        public string Message { get; set; }
        public Brush MessageColor { get; set; }

        // ========= COMMANDS =========
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // ========= CONSTRUCTOR =========
        public AddRecipeViewModel()
        {
            _recipeManager = App.SharedRecipeManager;
            _userManager = App.SharedUserManager;

            SaveCommand = new RelayCommand(SaveRecipe);
            CancelCommand = new RelayCommand(Cancel);
        }

        // ========= METHODS =========

        private async void SaveRecipe(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Title) ||
                string.IsNullOrWhiteSpace(Category) ||
                string.IsNullOrWhiteSpace(Ingredients) ||
                string.IsNullOrWhiteSpace(Instructions))
            {
                MessageBox.Show("Please fill in all fields.", "Empty input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var currentUser = _userManager.GetLoggedInUser();
            var ingredientsList = Ingredients
                .Split(new[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(i => i.Trim())
                .ToList();

            var newRecipe = new Recipe(Title, ingredientsList, Instructions, Category, currentUser);
            _recipeManager.AddRecipe(newRecipe);

            Message = "Recipe added successfully!";
            MessageColor = Brushes.DarkGreen;
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(MessageColor));

            await Task.Delay(1000);

            foreach (var w in Application.Current.Windows)
                if (w is AddRecipeWindow ar) ar.Close();
        }


        private void Cancel(object parameter)
        {
            foreach (var w in Application.Current.Windows)
                if (w is AddRecipeWindow ar) ar.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
