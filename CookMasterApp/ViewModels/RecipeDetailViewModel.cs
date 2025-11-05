using CookMasterApp.Managers;
using CookMasterApp.Models;
using CookMasterApp.Views;
using MVVM_KlonaMIg.MVVM;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CookMasterApp.ViewModels
{
    internal class RecipeDetailViewModel : INotifyPropertyChanged
    {
        private readonly RecipeManager _recipeManager;
        private readonly UserManager _userManager;
        private readonly Recipe _originalRecipe;

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }
        private bool _canEditControls;
        public bool CanEditControls
        {
            get => _canEditControls;
            set
            {
                _canEditControls = value;
                OnPropertyChanged();
            }
        }


        public bool IsReadOnly => !_isEditing;

        public string Title { get; set; }
        public string Category { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public string CreatedBy { get; }
        public DateTime Date { get; }

        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand BackCommand { get; }
        //Construktor
        public RecipeDetailViewModel(Recipe recipe)
        {
            _recipeManager = App.SharedRecipeManager;
            _userManager = App.SharedUserManager;
            _originalRecipe = recipe;

            Title = recipe.Title;
            Category = recipe.Category;
            Ingredients = string.Join(", ", recipe.Ingredients);
            Instructions = recipe.Instructions;
            CreatedBy = recipe.CreatedBy.Username;
            Date = recipe.Date;

            EditCommand = new RelayCommand(StartEditing);
            SaveCommand = new RelayCommand(SaveChanges, CanExecuteWhenEditing);
            CopyCommand = new RelayCommand(SaveAsNew, CanExecuteWhenEditing);
            BackCommand = new RelayCommand(BackToList);

        }

        //Methods
        private void StartEditing(object p)
        {
            IsEditing = true;
            CanEditControls = true;
        }
        private bool CanExecuteWhenEditing(object parameter)
        {
            return CanEditControls;
        }

        private bool IsValidInput ()
        {
            return ! string.IsNullOrWhiteSpace(Title)
                && ! string.IsNullOrWhiteSpace(Category)
                && ! string.IsNullOrWhiteSpace(Ingredients)
                && ! string.IsNullOrWhiteSpace(Instructions);
        }
        private void SaveChanges(object p)
        {
            _originalRecipe.EditRecipe(
                Title,
                Ingredients.Split(',', ';').Select(i => i.Trim()).ToList(),
                Instructions,
                Category);

            if (!IsValidInput())
            {
                MessageBox.Show("Please fill in all fields.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _recipeManager.UpdateRecipe(_originalRecipe);
            IsEditing = false;
            MessageBox.Show("Recipe updated successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            
            foreach (var w in Application.Current.Windows)
                if (w is RecipeDetailWindow rw)
                    rw.Close();
        }

        private void SaveAsNew(object p)
        {
            var currentUser = _userManager.GetLoggedInUser();
            if (string.IsNullOrWhiteSpace(Title) || _originalRecipe.Title == Title)
            {
                MessageBox.Show("Please enter a new title for the new recipe.", "Duplicate title", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!IsValidInput())
            {
                MessageBox.Show("Please fill in all fields.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            var newRecipe = new Recipe(
                Title,
                Ingredients.Split(',', ';').Select(i => i.Trim()).ToList(),
                Instructions,
                Category,
                currentUser);

            _recipeManager.AddRecipe(newRecipe);
            MessageBox.Show("Recipe copied and saved as new!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            foreach (var w in Application.Current.Windows)
                if (w is RecipeDetailWindow rw)
                    rw.Close();
        }

        private void BackToList(object p)
        {
            foreach (var w in Application.Current.Windows)
                if (w is RecipeDetailWindow rw) rw.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
