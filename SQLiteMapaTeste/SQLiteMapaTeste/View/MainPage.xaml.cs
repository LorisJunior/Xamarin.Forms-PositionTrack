using SQLite;
using SQLiteMapaTeste.Model;
using SQLiteMapaTeste.Service;
using SQLiteMapaTeste.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SQLiteMapaTeste
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            SetDefaultUser();
        }

        public void SetDefaultUser()
        {
            using (var db = new SQLiteConnection(App.DatabasePath))
            {
               // db.CreateTable<User>();

                var checkDb = db.Table<User>().ToList();
                
                if (checkDb.Count == 0)
                {
                    App.user.Nome = "New User";
                    App.user.Buffer = ImageService.ConvertImageToByte("SQLiteMapaTeste.Imagens.defaultProfileImage.png", App.assembly);
                    db.Insert(App.user);
                    countLabel.Text = db.Table<User>().ToList().Count.ToString();
                }
                else
                {
                    App.user = db.Table<User>().ToList().FirstOrDefault();
                    if (App.user.Nome == null)
                    {
                        App.user.Nome = "New User";
                    }
                    if (App.user.Buffer == null)
                    {
                        App.user.Buffer = ImageService.ConvertImageToByte("SQLiteMapaTeste.Imagens.defaultProfileImage.png", App.assembly);
                    }
                }
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabPage());
        }
    }
}
