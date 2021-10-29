using SQLite;
using SQLiteMapaTeste.Model;
using System;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SQLiteMapaTeste
{
    public partial class App : Application
    {
        public static Assembly assembly = null;
        public static User user = new User();
        public static string DatabasePath = string.Empty;
        public static string Key = "-Mmga23BdqP7Hp9-vkjV";
        public App()
        {
            assembly = GetType().GetTypeInfo().Assembly;

            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
           /*using (var db = new SQLiteConnection(DatabasePath))
            {
                db.DropTable<User>();
            }*/
        }

        public App(string databaseLocation)
        {
            assembly = GetType().GetTypeInfo().Assembly;

            InitializeComponent();

            DatabasePath = databaseLocation;
            MainPage = new NavigationPage(new MainPage());
          /*using (var db = new SQLiteConnection(databaseLocation))
            {
                db.DropTable<User>();
            }*/
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
