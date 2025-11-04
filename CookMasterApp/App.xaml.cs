using CookMasterApp.Managers;
using System.Configuration;
using System.Data;
using System.Windows;

namespace CookMasterApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UserManager SharedUserManager { get; } = new UserManager();
        public static RecipeManager SharedRecipeManager { get; } = new();
    }

}
