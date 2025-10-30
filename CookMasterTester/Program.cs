//using CookMasterApp.Managers;
//using CookMasterApp.Models;

//namespace CookMasterTester
//{
//    internal class Program
//    {
//        static void Main(string[] args)
//        {

//            Console.WriteLine("=== TESTAR COOKMASTER RECIPE ===\n");

//            // Skapa användare och manager
//            var user = new User("anna", "Password!1", "Sweden");
//            var recipeManager = new RecipeManager();

//            // Skapa första receptet
//            var recipe1 = new Recipe(
//                "Pannkakor",
//                new List<string> { "2 ägg", "2 dl mjölk", "1,5 dl mjöl", "Smör till stekning" },
//                "Vispa ihop och stek i smör.",
//                "Frukost",
//                user
//            );

//            recipeManager.AddRecipe(recipe1);

//            // Skapa ett till recept
//            var recipe2 = new Recipe(
//                "Spaghetti Bolognese",
//                new List<string> { "500 g nötfärs", "1 lök", "2 vitlöksklyftor", "400 g krossade tomater", "Spaghetti" },
//                "Stek lök och kött, tillsätt tomater. Servera med spaghetti.",
//                "Middag",
//                user
//            );

//            recipeManager.AddRecipe(recipe2);

//            // Visa alla recept
//            Console.WriteLine("Alla recept:");
//            foreach (var r in recipeManager.GetAllRecipes())
//            {
//                Console.WriteLine($"• {r.Title} ({r.Category}) – {r.IngredientsListText}");
//            }

//            // Testa CopyRecipe()
//            var copiedRecipe = recipe1.CopyRecipe();
//            copiedRecipe.Title = "Pannkakor Deluxe";
//            copiedRecipe.Ingredients.Add("1 msk vaniljsocker");
//            recipeManager.AddRecipe(copiedRecipe);

//            Console.WriteLine("\nEfter att ha kopierat receptet:");
//            foreach (var r in recipeManager.GetAllRecipes())
//            {
//                Console.WriteLine($"• {r.Title} ({r.Category}) – {r.IngredientsListText}");
//            }

//            // Testa Filter()
//            Console.WriteLine("\nSöker efter 'pann':");
//            var filtered = recipeManager.Filter("pann");
//            foreach (var r in filtered)
//            {
//                Console.WriteLine($"• {r.Title}");
//            }

//            Console.WriteLine("\nSöker efter 'middag':");
//            var filtered2 = recipeManager.Filter("middag");
//            foreach (var r in filtered2)
//            {
//                Console.WriteLine($"• {r.Title}");
//            }

//            // Testa UpdateRecipe()
//            recipe1.EditRecipe("Pannkakor med blåbär", recipe1.Ingredients, "Tillsätt blåbär i smeten.", recipe1.Category);
//            recipeManager.UpdateRecipe(recipe1);

//            Console.WriteLine("\nEfter att ha uppdaterat receptet:");
//            foreach (var r in recipeManager.GetAllRecipes())
//            {
//                Console.WriteLine($"• {r.Title} ({r.Category}) – {r.IngredientsListText}");
//            }
//            Console.WriteLine($"\n Ingrediants in recipe1: {recipe1.IngredientsListText}");

//            Console.WriteLine("\nTest klar — tryck valfri tangent för att avsluta.");
//            Console.ReadKey();

//        }
//    }
//}
