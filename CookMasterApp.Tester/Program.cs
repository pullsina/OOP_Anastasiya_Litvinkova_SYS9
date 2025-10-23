namespace CookMasterApp.Tester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using CookMasterApp.Managers;
            using CookMasterApp.Models;

            Console.WriteLine("=== TESTAR COOKMASTER ===");

            var manager = new UserManager();

            // --- Registrera användare ---
            Console.WriteLine("\nRegistrerar användare...");
            manager.Register("anna", "Password!1", "Sweden");
            manager.Register("bob", "Qwerty@2", "Norway");

            // --- Testa inloggning ---
            Console.WriteLine("\nFörsöker logga in som Anna...");
            bool login = manager.Login("anna", "Password!1");
            Console.WriteLine($"Inloggning lyckades: {login}");

            // --- Testa lösenordsbyte ---
            Console.WriteLine("\nFörsöker byta Annas lösenord...");
            bool changed = manager.ChangePassword("Password!1", "NewPass#2");
            Console.WriteLine($"Lösenordsbyte lyckades: {changed}");

            // --- Testa admin ---
            Console.WriteLine("\nSkapar admin och ändrar Bobs lösenord...");
            var admin = new AdminUser("admin", "Admin#1", "Sweden");
            bool adminChange = admin.ChangePasswordByAdmin(manager, "bob", "Reset#5");
            Console.WriteLine($"Admin ändrade bobs lösenord: {adminChange}");

            // --- Testa inloggning för Bob med nya lösenordet ---
            Console.WriteLine("\nFörsöker logga in som Bob med nya lösenordet...");
            bool bobLogin = manager.Login("bob", "Reset#5");
            Console.WriteLine($"Inloggning lyckades: {bobLogin}");

            Console.WriteLine("\nTest klar — tryck valfri tangent för att avsluta.");
            Console.ReadKey();

        }
    }
}
