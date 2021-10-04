using SQLiteMapaTeste.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SQLiteMapaTeste.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public User LocalUser { get; set; }
        public ChatPage()
        {
            InitializeComponent();
            BindingContext = this;
            if (LocalUser != null)
            {
                nome.Text = $"Nome: {LocalUser.Nome}";
                image.Source = ImageSource.FromStream(() => new MemoryStream(LocalUser.Buffer));
                latitude.Text = $"Latitude: {LocalUser.Latitude}";
                longitude.Text = $"Longitude: {LocalUser.Longitude}";
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (LocalUser != null)
            {
                nome.Text = $"Nome: {LocalUser.Nome}";
                image.Source = ImageSource.FromStream(() => new MemoryStream(LocalUser.Buffer));
                latitude.Text = $"Latitude: {LocalUser.Latitude}";
                longitude.Text = $"Longitude: {LocalUser.Longitude}";
            }
        }
    }
}