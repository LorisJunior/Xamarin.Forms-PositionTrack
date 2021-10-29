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
        FirebaseService fbService = new FirebaseService();
        byte[] buffer = null;

        User newUser = null;
        public AddUser()
        {
            InitializeComponent();
        }

        private async void AddUserClicked(object sender, EventArgs e)
        {
            newUser = new User();

            newUser.Buffer = buffer;
            newUser.Nome = nome.Text;
            newUser.Email = email.Text;
            newUser.Sobre = sobre.Text;
            newUser.Latitude = App.user.Latitude;
            newUser.Longitude = App.user.Longitude;
            newUser.DisplayUserInMap = true;
            newUser.Key = string.Empty;
            newUser.Items = new List<Item>
            {
                new Item
                {
                    Temp = 1,
                },
            };
            newUser.Notifications = new List<Notification>
            {
                new Notification
                {
                    Temp = 1,
                },
            };
            newUser.Conversas = new List<Conversa>
            {
                new Conversa
                {
                    Temp = 1,
                },
            };
            /*using (var db = new SQLiteConnection(App.DatabasePath))
            {
                db.CreateTable<User>();
                int row = db.Insert(newUser);
                labelCount.Text = db.Table<User>().ToList().Count.ToString();
            }*/
            await fbService.AddUser(newUser);
        }

        private async void AddFoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                var img = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = ""});
                var stream = await img.OpenReadAsync();
                buffer = ImageService.ConvertImageToByte(stream);
                foto.Source = ImageSource.FromStream(() => new MemoryStream(buffer));
            }
            catch (Exception)
            {
            }
        }
    }
}