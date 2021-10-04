using SQLite;
using SQLiteMapaTeste.Model;
using SQLiteMapaTeste.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SQLiteMapaTeste.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            {
                using (var db = new SQLiteConnection(App.DatabasePath))
                {
                    db.CreateTable<User>();
                    profileName.Text = App.user.Nome;
                    if (App.user.Buffer != null)
                    {
                        foto.Source = ImageSource.FromStream(() => new MemoryStream(App.user.Buffer));
                    }
                }
            }
        }
        private async void OnImageButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var img = await MediaPicker.PickPhotoAsync();
                var stream = await img.OpenReadAsync();
                App.user.Buffer = ImageService.ConvertImageToByte(stream);
                foto.Source = ImageSource.FromStream(() => new MemoryStream(App.user.Buffer));
            }
            catch (Exception)
            {
            }

            using (var db = new SQLiteConnection(App.DatabasePath))
            {
                db.CreateTable<User>();
                db.Update(App.user);
            }
        }
        private void OnNameButtonClicked(object sender, EventArgs e)
        {
            using (var db = new SQLiteConnection(App.DatabasePath))
            {
                App.user.Nome = entry.Text;
                db.CreateTable<User>();
                db.Update(App.user);
            }
            profileName.Text = App.user.Nome;
        }
    }
}