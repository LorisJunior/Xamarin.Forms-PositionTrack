using Plugin.Geolocator;
using SQLite;
using SQLiteMapaTeste.Model;
using SQLiteMapaTeste.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;
namespace SQLiteMapaTeste.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        Circle circle;
        Plugin.Geolocator.Abstractions.IGeolocator locator = null;
        Pin userPin;
        
        private double raio = 3000;
        public double Raio
        {
            get => raio;
            set
            {
                if (raio != value)
                {
                    raio = value;
                    OnPropertyChanged();
                    UpdateCircleRadius();
                }
            }
        }
        public MapPage()
        {
            circle = new Circle();
            InitializeComponent();
            BindingContext = this;

            map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;
            map.PinClicked += OnPinClicked;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //Pega a posição do usuário e começa a receber as atualizações de posição
            SetUserPin();

            locator = CrossGeolocator.Current;
            locator.PositionChanged += Locator_PositionChanged;
            await locator.StartListeningAsync(new TimeSpan(0, 0, 0), 100);
            var position = await locator.GetPositionAsync();
            using (var db = new SQLiteConnection(App.DatabasePath))
            {
                App.user.Latitude = position.Latitude;
                App.user.Longitude = position.Longitude;
                db.CreateTable<User>();
                db.Update(App.user);
            }
            var center = new Position(position.Latitude, position.Longitude);
            

            userPin.Position = center;
            CreateCircleShapeAt(center);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMeters(5000)), true);

        }
        protected async override void OnDisappearing()
        {
            base.OnDisappearing();

            if(map.Pins != null)
            {
                map.Pins.Clear();

            }
            //Para de receber as posições
            await locator.StopListeningAsync();
        }
        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var center = new Position(e.Position.Latitude, e.Position.Longitude);
            userPin.Position = center;
            CreateCircleShapeAt(center);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromMeters(5000)), true);
        }
        public void OnPinClicked(object sender, PinClickedEventArgs e)
        {
            Pin pin = e.Pin;
            int userId = (int)pin.Tag;
            using (var db = new SQLiteConnection(App.DatabasePath))
            {
                db.CreateTable<User>();
                var users = db.Table<User>().ToList();
                var user = users.Where(u => u.Id == userId).FirstOrDefault();

                //Envio uma mensagem para a TabPage ir para a ChatPage
                MessagingCenter.Send<object, int>(user, "click", 2);

            }
        }
        public void SetUserPin()
        {
            userPin = new Pin()
            {
                Icon = BitmapDescriptorFactory.FromStream(new MemoryStream(App.user.Buffer)),
                Type = PinType.Place,
                Label = "Olá, vms comprar juntos!",
                ZIndex = 5,
                Tag = App.user.Id
            };
            map.Pins.Add(userPin);
        }
        public void AddNearUsers()
        {
            using (var db = new SQLiteConnection(App.DatabasePath))
            {
                db.CreateTable<User>();
                var users = db.Table<User>().ToList().Where(u => u.Id != App.user.Id && 
                (DistanceService.CompareDistance(App.user.Latitude, App.user.Longitude, u.Latitude, u.Longitude) <= (Raio/1000)) );
                foreach (var user in users)
                {
                    Pin pin = new Pin()
                    {
                        Icon = BitmapDescriptorFactory.FromStream(new MemoryStream(user.Buffer)),
                        Type = PinType.Place,
                        Label = "Olá, vms comprar juntos!",
                        ZIndex = 5,
                        Position = new Position(user.Latitude, user.Longitude),
                        Tag = user.Id
                    };
                    map.Pins.Add(pin);
                }
            }
        }
        public void CreateCircleShapeAt(Position position)
        {
            if(map.Circles != null)
            {
                map.Circles.Clear();
            }

            circle.Center = position;
            circle.Radius = Distance.FromMeters(Raio);
            circle.StrokeColor = Color.DodgerBlue;
            circle.StrokeWidth = 6f;
            circle.FillColor = Color.FromRgba(0, 0, 255, 32);
            circle.Tag = "CIRCLE";

            map.Circles.Add(circle);
            
        }
        public void UpdateCircleRadius()
        {
            circle.Radius = Distance.FromMeters(Raio);
        }
        private void NearUsersClicked(object sender, EventArgs e)
        {
            if (map.Pins != null)
            {
                map.Pins.Clear();
            }
            map.Pins.Add(userPin);
            AddNearUsers();

        }
    }
}