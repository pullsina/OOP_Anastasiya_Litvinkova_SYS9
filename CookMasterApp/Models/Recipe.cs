using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMasterApp.Models
{
    public class Recipe
    {
        //properties
        public string Title { get; set; }
        public List<string> Ingredients { get; set; } = new();
        public string IngredientsListText => string.Join(", ", Ingredients);
        public string Instructions { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public User CreatedBy { get; set; }

        //Constructor
        public Recipe() { }
        public Recipe(string title, List <string> ingredients, string instructions, string category, User createdBy)
        {
            Title = title;
            Ingredients = ingredients ?? new List<string>();//använder listan om det finns annars skapar en ny
            Instructions = instructions;
            Category = category;
            CreatedBy = createdBy;
            Date = DateTime.Now;
        }


        //Methods 
        //EditRecipe()
        public void EditRecipe(string newTitle, List<string> newIngredients, string newInstructions, string newCategory)
        {
            Title = newTitle;
            Ingredients = newIngredients;
            Instructions = newInstructions;
            Category = newCategory;
            Date = DateTime.Now;
        }
        public void EditRecipe (Recipe editedRecipe)
        {
            Title = editedRecipe.Title;
            Ingredients = editedRecipe.Ingredients;
            Instructions = editedRecipe.Instructions;
            Category = editedRecipe.Category;
            Date = DateTime.Now;
        }
        public void EditRecipe ()
        {
            Date = DateTime.Now;
        }


        //CopyRecipy ()
        public Recipe CopyRecipe ()
        {
            return new Recipe(Title, new List <string> (Ingredients), Instructions, Category, CreatedBy);           

        }

    }
}
