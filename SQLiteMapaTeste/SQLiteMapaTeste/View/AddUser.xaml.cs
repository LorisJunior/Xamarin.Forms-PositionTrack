using SQLite;
using SQLiteMapaTeste.Model;
using SQLiteMapaTeste.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SQLiteMapaTeste.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddUser : ContentPage
    {
        User newUser = null;
        public AddUser()
        {
            InitializeComponent();
        }

        private async void AddUserClicked(object sender, EventArgs e)
        {
            newUser = new User();
            var random = new Random();
            int randomPosition = random.Next(0, 30);

            try
            {
                var img = await MediaPicker.PickPhotoAsync();
                var stream = await img.OpenReadAsync();
                newUser.Buffer = ImageService.ConvertImageToByte(stream);
                foto.Source = ImageSource.FromStream(() => new MemoryStream(newUser.Buffer));
            }
            catch (Exception)
            {
            }

            newUser.Nome = entry.Text;
            profileName.Text = newUser.Nome;
            newUser.Latitude = App.user.Latitude + randomPosition / 100;
            newUser.Longitude = App.user.Longitude + randomPosition / 95;


            using (var db = new SQLiteConnection(App.DatabasePath))
            {

                db.CreateTable<User>();
                int row = db.Insert(newUser);

                labelCount.Text = db.Table<User>().ToList().Count.ToString();
            }
        }
    }
}