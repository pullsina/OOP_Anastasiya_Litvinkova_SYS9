using CookMasterApp.Managers;
using CookMasterApp.Models;

namespace CookMasterTester
{
    internal class Program
    {
        static void Main(string[] args)
        {        

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

            Console.WriteLine("\nFörsöker logga in som Anna...");
            bool login2 = manager.Login("anna", "NewPass#2");
            Console.WriteLine($"Inloggning lyckades: {login2}");

            // logga in som admin
            bool adminLogin = manager.Login("admin", "password");
            Console.WriteLine($"Admin inloggad: {adminLogin}");

            // hämta den inloggade användaren
            var loggedAdmin = manager.GetLoggedInUser();
            Console.WriteLine($"Inloggad användare: {loggedAdmin?.Username}");

            // ändra en annan användares lösenord
            if (loggedAdmin is AdminUser adminUser)
            {
                bool changed2 = adminUser.ChangePasswordByAdmin(manager, "user", "Reset#55!");
                Console.WriteLine($"Admin ändrade user-lösenord: {changed2}");
            }
            manager.Logout();
            bool userLogin = manager.Login("user", "Reset#55!");
            Console.WriteLine($"User kan logga in med nytt lösenord: {userLogin}");



            Console.WriteLine("\nTest klar — tryck valfri tangent för att avsluta.");
            Console.ReadKey();

        }
    }
}
