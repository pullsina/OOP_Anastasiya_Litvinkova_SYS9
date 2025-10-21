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
        public string Ingrediants { get; set; }
        public string Instruction { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public User CreatedBy { get; set; }


        //Methods 
        //EditRecipe()
        //CopyRecipy ()
    }
}
