using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMasterApp.Models
{
    internal class Recipe
    {
        //properties
        public string Title { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public User CreatedBy { get; set; }

        //Constructor
        public Recipe(string title, string ingredients, string instructions, string category, User createdBy)
        {
            Title = title;
            Ingredients = ingredients;
            Instructions = instructions;
            Category = category;
            CreatedBy = createdBy;
            Date = DateTime.Now;
        }


        //Methods 
        //EditRecipe()
        public void EditRecipe(string newTitle, string newIngredients, string newInstructions, string newCategory)
        {
            Title = newTitle;
            Ingredients = newIngredients;
            Instructions = newInstructions;
            Category = newCategory;
            Date = DateTime.Now;
        }

        //CopyRecipy ()
        public Recipe CopyRecipy ()
        {
            return new Recipe(Title, Ingredients, Instructions, Category, CreatedBy);
        }

    }
}
